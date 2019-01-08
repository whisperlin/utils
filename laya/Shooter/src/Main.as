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
			
			this.scene = Laya.stage.addChild(new Scene3D()) as Scene3D;
			
			var camera:Camera = (scene.addChild(new Camera(0, 0.1, 1000))) as Camera;
			camera.clearColor = new Vector4(0.5,0.5,0.5,1);
			camera.transform.translate(new Vector3(0, 1.5, 4));
			camera.transform.rotate(new Vector3(-15, 0, 0), true, false);
	 
			
			var directionLight:DirectionLight = scene.addChild(new DirectionLight()) as DirectionLight;
			directionLight.transform.worldMatrix.setForward(new Vector3(-1.0, -1.0, -1.0));
			directionLight.color = new Vector3(1, 1, 1);
			 
			
			var resource:Array = [
				 {url: "Role/Role.lh", type: Laya3D.HIERARCHY, priority: 1},
				 {url: "Role/water.lh", type: Laya3D.HIERARCHY, priority: 1}
				 
				
			];
			
			Laya.loader.create(resource, Handler.create(this, onLoadFinish));
		}
		
		private function onLoadFinish():void {
	 
			//var hand:Sprite3D = Laya.loader.getRes("RoleDemo/hand.lh");
			//this.scene.addChild(hand);
			//var gun:Sprite3D  = Laya.loader.getRes("test2/LayaScene_Role/Conventional/Role.lh");
			var gun:Sprite3D  = Laya.loader.getRes("Role/Role.lh");
			
			this.scene.addChild(gun);

			var water:Sprite3D  = Laya.loader.getRes("Role/water.lh");
			this.scene.addChild(water);
			//gun.transform.position=new Laya.Vector3(5, 0, 0);
			gun.transform.rotate(new Vector3(0,50,0),false);
			
			var ani:Animator = gun.getChildAt(0).getComponent(Animator);
			
	 
			//ani.scriptRotationNodeIndex = 3;
			//Quaternion.createFromYawPitchRoll(0, 0 ,-20*0.017, ani.scriptRatation );
			//var lay = ani.getlayer(0);
			//lay.defaultWeight = 0;
			
			ani.play("stand",0)
			//ani.play("huandanjia",1)
			
			//ani.play("attack",1)
			var defaultState :AnimatorState;
			defaultState =  ani.getDefaultState(0);
			
			//获取旋转节点所在的索引.
			var nodes:KeyframeNodeList = defaultState._clip._nodes;
	 		var nodeOwners:Vector.<KeyframeNodeOwner> = defaultState._nodeOwners;
			for (var i:int = 0, n:int = nodes.count; i < n; i++) {
				var nodeOwner:KeyframeNodeOwner = nodeOwners[i];
				//console.log(nodeOwner.fullPath);
				if(nodeOwner.fullPath == "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1.transform.localRotation")
				{
					//这个为扩展属性.
					ani.scriptRotationNodeIndex = i;
					break;
				}
			}


			//ani.getControllerLayer(0);
			var angle : Number;
			var angle2 : Number;
			var dir : int;
			var dir2 : int;
			var deltaAngle : Number;
			var deltaAngle2 : Number;
			angle = 0;
			angle2 = 0;
			dir = -1;
			dir2 = -1;
			deltaAngle = 0.5;
			deltaAngle2 = 2;
			
			
			Laya.timer.loop(10,null,function():void{
				angle = angle + deltaAngle*dir;
				angle2 = angle2 + deltaAngle2*dir2;
				if(angle < -30)
				{
					dir = 1;
					ani.play("stand",0);
				}
				if(angle > 50)
				{
					ani.play("walk",0);
					dir = -1;
				}

				if(angle2 < -70)
				{
					dir2 = 1;
					ani.play("attack",1);
				}
				if(angle2 > 70)
				{
					dir2 = -1;
					ani.play("huandanjia",1);
				}
				//console.log(angle2);

				//0.017是角度转弧度的大约转化率.
				//这个也为扩展属性。
				Quaternion.createFromYawPitchRoll(0, angle2*0.017 ,angle*0.017, ani.scriptRatation );
            })
			ani.play("kaiqiang",0)
			console.log("LoadResCompleted");
			//ani2.play("naqi",1)

		}
	
	}
}