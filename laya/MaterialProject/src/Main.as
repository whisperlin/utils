package  {
 
	import laya.d3.component.Animator;
	import laya.d3.component.AnimatorState;
	import laya.d3.core.Camera;
	import laya.d3.core.Sprite3D;
	import laya.d3.core.light.DirectionLight;
	import laya.d3.core.scene.Scene3D;
	import laya.d3.math.Vector3;
	import laya.display.Stage;
	import laya.events.Event;
	import laya.ui.Button;
	import laya.utils.Browser;
	import laya.utils.Handler;
	import laya.utils.Stat;
	import laya.d3.math.Vector4;
	import laya.d3.math.Quaternion;
	import laya.d3.animation.KeyframeNodeList;
	import laya.d3.component.KeyframeNodeOwner;
	/**
	 * ...
	 * @author
	 */
	public class Main {
		private var changeActionButton:Button;
		private var zombieAnimator:Animator;
		private  var scene:Scene3D
		private var curStateIndex:int = 0;
		private var clipName:Array = ["idle", "fallingback", "idle", "walk", "Take 001"];
		
		public function Main() {
			Laya3D.init(0, 0);
			Laya.stage.scaleMode = Stage.SCALE_FULL;
			Laya.stage.screenMode = Stage.SCREEN_NONE;
			Stat.show();
 
			var resource:Array = [
				 {url: "LayaScene_water/Conventional/water.ls", type: Laya3D.HIERARCHY, priority: 1}
			];
			
			Laya.loader.create(resource, Handler.create(this, onLoadFinish));
		}
		
		private function onLoadFinish():void {
	 
			 
			this.scene  = Laya.loader.getRes("LayaScene_water/Conventional/water.ls") as Scene3D;

			Laya.stage.addChild(this.scene);
			 
			 

		}
	
	}
}