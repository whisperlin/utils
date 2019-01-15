package script {
	import laya.components.Prefab;
	import laya.components.Script;
	import laya.display.Sprite;
	import laya.events.Event;
	import laya.utils.Browser;
	import laya.utils.Pool;
	
	/**
	 * 游戏控制脚本。定义了几个dropBox，bullet，createBoxInterval等变量，能够在IDE显示及设置该变量
	 * 更多类型定义，请参考官方文档
	 */
	public class GameControl extends Script {
		/** @prop {name:dropBox,tips:"掉落容器预制体对象",type:Prefab}*/
		public var dropBox:Prefab;
		/** @prop {name:bullet,tips:"子弹预制体对象",type:Prefab}*/
		public var bullet:Prefab;
		/** @prop {name:createBoxInterval,tips:"间隔多少毫秒创建一个下跌的容器",type:int,default=1000}*/
		public var createBoxInterval:Number = 1000;
		/**开始时间*/
		private var _time:Number = 0;
		/**是否已经开始游戏 */
		private var _started:Boolean = false;
		/**子弹和盒子所在的容器对象 */
		private var _gameBox:Sprite;
		
		override public function onEnable():void {
			this._time = Browser.now();
			this._gameBox = this.owner.getChildByName("gameBox") as Sprite;
			this.createBox();
		}
		
		override public function onUpdate():void {
			//每间隔一段时间创建一个盒子
			var now:* = Browser.now();
			if (now - this._time > this.createBoxInterval) {
				this._time = now;
				this.createBox();
			}
		}
		
		public function createBox():void {
			//使用对象池创建盒子
			var box:Sprite = Pool.getItemByCreateFun("dropBox", this.dropBox.create, this.dropBox);
			box.pos(Math.random() * (Laya.stage.width - 100), -100);
			this._gameBox.addChild(box);
		}
		
		override public function onStageClick(e:Event):void {
			//停止事件冒泡，提高性能，当然也可以不要
			e.stopPropagation();
			//舞台被点击后，使用对象池创建子弹
			var flyer:Sprite = Pool.getItemByCreateFun("bullet", this.bullet.create, this.bullet);
			flyer.pos(Laya.stage.mouseX, Laya.stage.mouseY);
			this._gameBox.addChild(flyer);
		}
		
		/**开始游戏，通过激活本脚本方式开始游戏*/
		public function startGame():void {
			if (!this._started) {
				this._started = true;
				this.enabled = true;
			}
		}
		
		/**结束游戏，通过非激活本脚本停止游戏 */
		public function stopGame():void {
			this._started = false;
			this.enabled = false;
			this.createBoxInterval = 1000;
			this._gameBox.removeChildren();
		}
	}
}