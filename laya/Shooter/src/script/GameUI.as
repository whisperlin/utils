package script {
	import laya.events.Event;
	import laya.events.MouseManager;
	import ui.test.TestSceneUI;
	import laya.d3.core.scene.Scene3D;
	import laya.d3.core.Camera;
	import laya.d3.math.Vector3;
	import laya.d3.core.light.DirectionLight;
	import laya.d3.core.MeshSprite3D;
	import laya.d3.resource.models.BoxMesh;
	import laya.d3.core.material.BlinnPhongMaterial;
	import laya.webgl.resource.Texture2D;
	import laya.utils.Handler;
	import laya.display.Stage;
	
	/**
	 * 本示例采用非脚本的方式实现，而使用继承页面基类，实现页面逻辑。在IDE里面设置场景的Runtime属性即可和场景进行关联
	 * 相比脚本方式，继承式页面类，可以直接使用页面定义的属性（通过IDE内var属性定义），比如this.tipLbll，this.scoreLbl，具有代码提示效果
	 * 建议：如果是页面级的逻辑，需要频繁访问页面内多个元素，使用继承式写法，如果是独立小模块，功能单一，建议用脚本方
	 */
	public class GameUI extends TestSceneUI {
		public function GameUI():void {
			super();
		
			//添加3D场景
			var scene:Scene3D = Laya.stage.addChild(new Scene3D()) as Scene3D;
			
			//添加照相机
			var camera:Camera = (scene.addChild(new Camera(0, 0.1, 100))) as Camera;
			camera.transform.translate(new Vector3(0, 3, 3));
			camera.transform.rotate(new Vector3(-30, 0, 0), true, false);
			
			//添加方向光
			var directionLight:DirectionLight = scene.addChild(new DirectionLight()) as DirectionLight;
			directionLight.color = new Vector3(0.6, 0.6, 0.6);
			directionLight.transform.worldMatrix.setForward(new Vector3(1, -1, 0));
			
			//添加自定义模型
			var box:MeshSprite3D = scene.addChild(new MeshSprite3D(new BoxMesh(1, 1, 1))) as MeshSprite3D;
			box.transform.rotate(new Vector3(0, 45, 0), false, false);
			var material:BlinnPhongMaterial = new BlinnPhongMaterial();
			Texture2D.load("res/layabox.png", Handler.create(null, function(tex:Texture2D):void {
				material.albedoTexture = tex;
			}));
			box.meshRenderer.material = material;
		}
	}
}