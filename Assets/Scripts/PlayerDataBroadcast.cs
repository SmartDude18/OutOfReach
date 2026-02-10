using UnityEngine;

public class PlayerDataBroadcast : MonoBehaviour
{
    public void PlayerMove(bool isMove, float moveMagnitude)
    {
        if(isMove)
        {
            //Debug.Log("MOVE BROADCAST");
            //broadcast for moving
        }
        else
        {
            //Debug.Log("STILL BROADCAST");
            //broadcast for not moving
        }
    }

    public void PlayerJump()
    {
        //Debug.Log("JUMP BROADCAST");
        //broadcast for jumping
    }

    public void PlayerGrounded(bool isGrounded)
    {
        if( isGrounded)
        {
            //Debug.Log("GROUND BROADCAST");
            //broadcast for player on ground
        }
        else
        {
            //Debug.Log("AIR BROADCAST");
            //broadcast for player off ground
        }
    }
}
