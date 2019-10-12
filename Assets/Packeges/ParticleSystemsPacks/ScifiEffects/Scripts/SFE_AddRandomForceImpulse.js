#pragma strict

var rnd:float=10;

function Start () {
GetComponent.<Rigidbody>().AddForce(Random.Range(-rnd, rnd), Random.Range(-rnd, rnd), Random.Range(-rnd, rnd), ForceMode.Impulse);
}

function Update () {

}