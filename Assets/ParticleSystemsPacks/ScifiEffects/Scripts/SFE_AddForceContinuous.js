#pragma strict

var force:float=10;

function Start () {

}

function Update () {
GetComponent.<Rigidbody>().AddForce(transform.up * force*Time.deltaTime, ForceMode.Impulse);
}