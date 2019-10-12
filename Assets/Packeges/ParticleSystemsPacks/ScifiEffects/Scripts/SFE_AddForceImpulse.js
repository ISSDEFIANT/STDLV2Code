#pragma strict

function Start () {
GetComponent.<Rigidbody>().AddForce(transform.up * 10, ForceMode.Impulse);
}

function Update () {

}