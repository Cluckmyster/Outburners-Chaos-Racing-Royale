using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitpoints : MonoBehaviour
{
    CarController carController;
    AIController aiController;
    bool isAI;

    public bool cooldown;
    private bool coolingDown;
    public int hitpoints = 1000;
    public int damage;
    public float damageMultiplier = 1.0f;

    //Start is called on the first frame
    private void Start()
    {
        //Find Controller script
        aiController = gameObject.GetComponent<AIController>();
        isAI = true;
        if (aiController == null)
        {
            carController = gameObject.GetComponent<CarController>();
            isAI = false;
        }
    }

    //Update is called once per frame
    private void Update()
    {
        if (cooldown && !coolingDown)
        {
            StartCoroutine(Cooldown());
        }

        //Calculate damage at current speed
        if (isAI)
        {
            damage = Mathf.RoundToInt(damageMultiplier * aiController.currentSpeed);
        }
        else
        {
            damage = Mathf.RoundToInt(damageMultiplier * carController.currentSpeed);
        }
        //Cap damage to prevent one-shotting
        if (damage > 600)
        {
            damage = 600;
        }

        //Death
        if (hitpoints <= 0)
        {
            if (isAI)
            {
                aiController.enabled = false;
            }
            else
            {
                carController.enabled = false;
            }
            this.enabled = false;
        }
    }

    //Taking damage on collision with car bumpers
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AICar"))
        {
            Hitpoints otherScript = other.gameObject.transform.parent.GetComponent<Hitpoints>();
            hitpoints = hitpoints - otherScript.damage;

            //Audio
            other.transform.gameObject.GetComponent<AudioSource>().Play();

            //Setting cooldown on so they can't repeatedly damage a car in melee
            if (otherScript.cooldown == false)
            {
                otherScript.cooldown = true;
            }

            //Reset AI target
            if (other.CompareTag("AICar"))
            {
                other.gameObject.transform.parent.GetComponent<AIController>().target = null;
            }
        }

        Debug.Log(hitpoints);
    }

    IEnumerator Cooldown()
    {
        coolingDown = true;
        yield return new WaitForSeconds(2);
        cooldown = false;
        coolingDown = false;
    }
}
