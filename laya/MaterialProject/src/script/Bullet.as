package script {
	import laya.components.Script;
	import laya.display.Sprite;
	import laya.physics.RigidBody;
	import laya.utils.Pool;
	
	/**
	 * 子弹脚本，实现子弹飞行逻辑及对象池回收机制
	 */
	public class Bullet extends Script {
		
		override public function onEnable():void {
			//设置初始速度
			var rig:RigidBody = this.owner.getComponent(RigidBody);
			rig.setVelocity({x: 0, y: -10});
		}
		
		override public function onTriggerEnter(other:*,self:*,contact:*):void {
			//如果被碰到，则移除子弹
			this.owner.removeSelf();
		}
		
		override public function onUpdate():void {
			//如果子弹超出屏幕，则进行移除子弹
			if ((this.owner as Sprite).y < -10) {
				this.owner.removeSelf();
			}
		}
		
		override public function onDisable():void {
			//子弹被移除时，回收子弹到对象池，方便下次复用，减少对象创建开销
			Pool.recover("bullet", this.owner);
		}
	}
}