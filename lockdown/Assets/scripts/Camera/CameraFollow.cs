//Author @ ANUJ SHRESTHA
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	private GameObject player;
	public Vector3 Offset;
	public float smoother;
	public bool verticalMove,horizontalMove;
    public Vector2 topRightCorner,bottomLeftCorner;
	void Start () {
        player = gameData.player;
        transform.position = player.transform.position;
	}
	void FixedUpdate () {
		if (player != null) {
            bool PlayerFacingForward = player.GetComponent<PlayerController>().facingRight;
            Vector2 movePoint = new Vector3(player.transform.position.x + (PlayerFacingForward ? Offset.x : -Offset.x), Offset.y+player.transform.position.y);
            movePoint.x =horizontalMove? Mathf.Clamp(movePoint.x, bottomLeftCorner.x, topRightCorner.x):transform.position.x;
            movePoint.y =verticalMove? Mathf.Clamp(movePoint.y, bottomLeftCorner.y, topRightCorner.y):transform.position.y;
            Vector3 playerPosition =new Vector3(movePoint.x,movePoint.y, player.transform.position.z +Offset.z);
			gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, playerPosition , smoother / 10);
		}
}
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(topRightCorner, .1f);
        Gizmos.DrawSphere(bottomLeftCorner, .1f);
    }
}