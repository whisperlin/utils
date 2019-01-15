package script {
	import laya.components.Script;
	import laya.display.Animation;
	import laya.display.Sprite;
	import laya.display.Text;
	import laya.events.Event;
	import laya.media.SoundManager;
	import laya.physics.RigidBody;
	import laya.utils.Pool;
	
	/**
	 * 掉落盒子脚本，实现盒子碰撞及回收流程
	 */
	public class DropBox extends Script {
		/**盒子等级 */
		public var level:Number = 1;
		/**等级文本对象引用 */
		private var _text:Text;
		/**刚体对象引用 */
		private var _rig:RigidBody;
		
		override public function onEnable():void {
			/**获得组件引用，避免每次获取组件带来不必要的查询开销 */
			this._rig = this.owner.getComponent(RigidBody);
			this.level = Math.round(Math.random() * 5) + 1;
			this._text = this.owner.getChildByName("levelTxt") as Text;
			this._text.text = this.level + "";
		}
		
		override public function onUpdate():void {
			//让持续盒子旋转
			(this.owner as Sprite).rotation++;
		}
		
		override public function onTriggerEnter(other:*,self:*,contact:*):void {
			var owner:Sprite = this.owner as Sprite;
			if (other.label === "buttle") {
				//碰撞到子弹后，增加积分，播放声音特效
				if (this.level > 1) {
					this.level--;
					this._text.changeText(this.level + "");
					owner.getComponent(RigidBody).setVelocity({x: 0, y: -10});
					SoundManager.playSound("sound/hit.wav");
				} else {
					if (owner.parent) {
						var effect:Animation = Pool.getItemByCreateFun("effect", this.createEffect, this);
						effect.pos(owner.x, owner.y);
						owner.parent.addChild(effect);
						effect.play(0, true);
						owner.removeSelf();
						SoundManager.playSound("sound/destroy.wav");
					}
				}
				GameUI.instance.addScore(1);
			} else if (other.label === "ground") {
				//只要有一个盒子碰到地板，则停止游戏
				owner.removeSelf();
				GameUI.instance.stopGame();
			}
		}
		
		/**使用对象池创建爆炸动画 */
		public function createEffect():Animation {
			var ani:Animation = new Animation();
			ani.loadAnimation("test/TestAni.ani");
			ani.on(Event.COMPLETE, null, recover);
			function recover():void {
				ani.removeSelf();
				Pool.recover("effect", ani);
			}
			return ani;
		}
		
		override public function onDisable():void {
			//盒子被移除时，回收盒子到对象池，方便下次复用，减少对象创建开销。
			Pool.recover("dropBox", this.owner);
		}
	}
}