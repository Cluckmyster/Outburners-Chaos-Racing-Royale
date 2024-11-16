using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarColour : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material red;
    [SerializeField] private Material purple;
    [SerializeField] private Material blue;
    [SerializeField] private Material green;
    [SerializeField] private Material yellow;
    [SerializeField] private Material orange;
    public Material[] playerColours;
    public int currentColourIndex = 0;

    private PlayerObjectController clientObjectController;
    private TMP_Dropdown colourDropdown;

    // Start is called before the first frame update
    void Start()
    {
        //clientObjectController = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>();
        colourDropdown = gameObject.GetComponent<TMP_Dropdown>();

        //colourDropdown.onValueChanged.AddListener(delegate { clientObjectController.cmdUpdateCar(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (clientObjectController == null)
        {
            clientObjectController = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>();
            colourDropdown.onValueChanged.AddListener(delegate { clientObjectController.cmdUpdateCar(currentColourIndex); });
            //colourDropdown.onValueChanged.AddListener(delegate { clientObjectController.UpdateCarCosmetics(); });
        }

        colourDropdown = gameObject.GetComponent<TMP_Dropdown>();
    }

    public void SetCarColour()
    {
        currentColourIndex = colourDropdown.value;
        /*
        if (colourDropdown.value == 0)
        {
            clientObjectController.newColour = red;
        }
        else if (colourDropdown.value == 1)
        {
            clientObjectController.newColour = purple;
        }
        else if (colourDropdown.value == 2)
        {
            clientObjectController.newColour = blue;
        }
        else if (colourDropdown.value == 3)
        {
            clientObjectController.newColour = green;
        }
        else if (colourDropdown.value == 4)
        {
            clientObjectController.newColour = yellow;
        }
        else if (colourDropdown.value == 5)
        {
            clientObjectController.newColour = orange;
        }
        */
    }
}
