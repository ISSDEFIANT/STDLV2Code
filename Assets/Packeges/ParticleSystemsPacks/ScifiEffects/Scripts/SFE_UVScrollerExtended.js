//This script scrolls the UV on an object (by offsetting its material).


var velocityY:float = 0.0;
var velocityX:float = 0.5;
var matNumber:int=0;

function Start()
{	
	if ( GetComponent.<Renderer>() )
		enabled = false;
}

function Update() 
{
	GetComponent.<Renderer>().materials[matNumber].mainTextureOffset.y += velocityY * Time.deltaTime;
	GetComponent.<Renderer>().materials[matNumber].mainTextureOffset.x += velocityX * Time.deltaTime;
}

function OnBecameVisible()
{
	this.enabled = true;
}

function OnBecameInvisible()
{
	this.enabled = false;
}
