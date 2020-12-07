using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour

{
    private RubyController rubyController;
    
    void OnTriggerStay2D(Collider2D other)
    {
        RubyController player = other.GetComponent<RubyController>();

        if (player.shield == true)
        {
            player.ChangeHealth(0);
        }
        
        else if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
}
