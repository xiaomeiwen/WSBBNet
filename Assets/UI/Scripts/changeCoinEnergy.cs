using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class changeCoinEnergy : MonoBehaviour {
	public Text coin;
	public Text energy;
	int coinVal;
	int energyVal;

	void Start() {
		coinVal = 3999;
		energyVal = 3999;
		coin.text = coinVal.ToString();
		energy.text = energyVal.ToString();
	}

	public void changeCoinVal() {
		coinVal++;
		coin.text = coinVal.ToString();
	}

	public void changeEnergyVal() {
		energyVal++;
		energy.text = energyVal.ToString();
	}
}
