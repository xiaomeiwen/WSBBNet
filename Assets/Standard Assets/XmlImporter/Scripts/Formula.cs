using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Formula
{
	public static void changePlayerExpBarLength(RectTransform expBar, float expLen, float actualLen, float expValue)
    {
		
		float newWidth = expValue * expLen / actualLen;   // expLen is the total length of the bar
//		Debug.Log ("expValue: " + expValue + " newWidth: " + newWidth);
		expBar.sizeDelta = new Vector2 (newWidth, expBar.rect.height);
    }

    public static void changeTeamBarLength(RectTransform speed_img, RectTransform agility_img, RectTransform power_img,
                                            float totalSpeed,float totalAgility, float totalPower){
        float sWidth, pWidth, aWidth;

        sWidth = speed_img.rect.width;
		aWidth = agility_img.rect.width;
        pWidth = power_img.rect.width;
        
		float sxPos = speed_img.localPosition.x;
		// *2 because the total length of the bar is 600, and the sum of speed, agility and power is 300
		float newSWidth = totalSpeed * 2; 
		float axPos = sxPos + newSWidth;
		float newAWidth = totalAgility * 2;
		float pxPos = axPos + newAWidth;
		float newPWidth = totalPower * 2;

		speed_img.sizeDelta = new Vector2 (newSWidth, speed_img.rect.height);

		agility_img.sizeDelta = new Vector2 (newAWidth, agility_img.rect.height);
		agility_img.localPosition = new Vector3 (axPos, agility_img.localPosition.y);

		power_img.sizeDelta = new Vector2 (newPWidth, power_img.rect.height);
		power_img.localPosition = new Vector3 (pxPos, power_img.localPosition.y);

//		Debug.Log ("speedPos: " + speed_img.localPosition.x + "agilityPos: " + agility_img.localPosition.x + "powerPos: " + power_img.localPosition.x);
//		Debug.Log ("sWidth: " + speed_img.rect.width + "aWidth: " + agility_img.rect.width + "pWidth: " + power_img.rect.width);
  }

    public static void resetBars(GameObject agility, GameObject speed, GameObject power,
                                    float agilityDefaultXPos, float speedDefaultXPos, float powerDefaultXPos)
    {
        RectTransform agilityRect = agility.GetComponent<RectTransform>();
        agilityRect.position = new Vector3(agilityDefaultXPos, agilityRect.position.y, 0);
        RectTransform speedRect = speed.GetComponent<RectTransform>();
        speedRect.position = new Vector3(speedDefaultXPos, speedRect.position.y, 0);
        RectTransform powerRect = power.GetComponent<RectTransform>();
        powerRect.position = new Vector3(powerDefaultXPos, powerRect.position.y, 0);
    }
    public static void resetTotalBars(RectTransform speed_image, RectTransform agility_image, RectTransform power_image,
                        float defaultWidth, float defaultPowerXPos, float defaultAgilityXPos)
    {
        speed_image.sizeDelta = new Vector2(defaultWidth, speed_image.rect.height);
        power_image.sizeDelta = new Vector2(defaultWidth, power_image.rect.height);
        agility_image.sizeDelta = new Vector2(defaultWidth, agility_image.rect.height);
        power_image.localPosition = new Vector3(defaultPowerXPos, power_image.localPosition.y);
        agility_image.localPosition = new Vector3(defaultAgilityXPos, agility_image.localPosition.y);
    }

}
