using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> healthIcons = new List<GameObject>();

    [SerializeField] private GameObject heartSprite;

    //max # of hearts in row before adding the next "layer"(new color)
    [SerializeField] private int maxHeartRow = 10;
    
    //list of all heart color layers, with 0 = empty
    [SerializeField] private List<Sprite> healthSprites = new List<Sprite>();

    public float GetShowableHearts(){
        return (float)maxHeartRow * healthSprites.Count;
    }

    public void UpdateHealthUI(int hearts)
    {
        //max number of hearts allowed
        if(hearts > (maxHeartRow*(healthSprites.Count-1))){
            hearts = maxHeartRow*(healthSprites.Count-1);
            Debug.LogWarning("UI Maximum execeed, Player has more health then shown");
        }


        //if there are more hearts then icons, create missing icons
        if (hearts > healthIcons.Count && healthIcons.Count < maxHeartRow){
            
            int difference = 0;

            //amount of new hearts need to make
            if(hearts > maxHeartRow){
                difference = maxHeartRow - healthIcons.Count;
            }else{
                difference = hearts - healthIcons.Count;
            }
            
            //make hearts
            for (int i = 0; i < difference; i++){
                healthIcons.Add(Instantiate(heartSprite, gameObject.transform));
            }
        }

        if (healthIcons == null || healthIcons.Count == 0)
            return;

        

        //check which layer all hearts must be to match hp value(i.e 22 hearts must be a min of 2 layers assumeing 10 is a row)
        int currentLayer = (int)Mathf.Floor(hearts/maxHeartRow);

        //check if currentlayer is in index bounds
        if (currentLayer > (healthSprites.Count - 1) || currentLayer < 0){
            Debug.LogWarning("health UI out of bounds, didn't update");
            return;
        }

        //replace heart sprites with the correct layer color
        for(int i = 0; i < healthIcons.Count; i++){
            if(i < (hearts%maxHeartRow)){
                //checks if heart needs to be a layer higher then the rest
                healthIcons[i].GetComponent<Image>().sprite = healthSprites[currentLayer + 1];
            }
            else{
                healthIcons[i].GetComponent<Image>().sprite = healthSprites[currentLayer];
            } 
        }


        /*for (int i = 0; i < healthIcons.Count; i++) // currently set up for max 8 hearts, can switch to instantiated prefabs if needed
        {
            if (healthIcons[i] != null)
            {
                if (healthIcons[i].activeSelf != i < hearts) // only change if the prev state needs to be updated
                    healthIcons[i].SetActive(i < hearts);
            }
        }*/
    }
}
