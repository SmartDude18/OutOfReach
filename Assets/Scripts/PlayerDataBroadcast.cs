using UnityEngine;

public class PlayerDataBroadcast : MonoBehaviour
{
    [SerializeField] AudioSource narrator;
    [SerializeField] AudioClip spikeDeathClip;
    [SerializeField] AudioClip pitDeathClip;

    [SerializeField] UIBehavior uiBehavior;

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

    public void PlayerDies(string tag)
    {
        switch(tag)
        {
            case "spike":
                //audio for spike death
                Debug.Log("Spike Death");
                if (spikeDeathClip != null)
                {
                    narrator?.PlayOneShot(spikeDeathClip);
                }
                break;
            case "DeathBox":
                //audio for falling off
                Debug.Log("Fall death");
                if (pitDeathClip != null)
                {
                    narrator?.PlayOneShot(pitDeathClip);
                }
                break;
            case "badCheckpoint":
                //audio for hitting bad checkpoint
                Debug.Log("Bad Checkpoint");
                break;
        }
        uiBehavior?.OnDeath();
    }
}
