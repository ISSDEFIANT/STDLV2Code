#pragma strict

var rnd:float=10;

function Start () {
//rigidbody.AddForce(transform.right * Random.Range(-rnd, rnd), ForceMode.Impulse);
GetComponent.<Rigidbody>().AddTorque (Random.Range(-rnd, rnd), Random.Range(-rnd, rnd), Random.Range(-rnd, rnd), ForceMode.Impulse);
}

function Update () {

}