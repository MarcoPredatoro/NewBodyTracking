using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    // Start is called before the first frame update

    private int points;
    public Text pointsText;
    public UnityEngine.UI.Image problemImage;
    public Material problemMaterial;
    public Material reset;
    public int getPoints() {
        return points;
    }
    public void updatePoints(int value) {
        points += value;
        pointsText.text = "Points: " + points.ToString();
    }

    private bool loseTimer = false;
    public void losePoints(int value) {
        if (!loseTimer) {
            loseTimer = true;
            points -= value;
            pointsText.text = "Points: " + points.ToString();
            problemImage.color = new Color(255,0,0);
            problemImage.material = problemMaterial;
            StartCoroutine(turnBacktoWhite());

        }
    }

    private bool stab = false;
    public void marcoCollision()
    {
        if (!stab)
        {
            stab = true;
            problemImage.color = new Color(255, 0, 0);
            problemImage.material = problemMaterial;
            GetComponent<EventManager>().SendMarcoCollision();
            StartCoroutine(turnBacktoWhite());
        }
    }

    IEnumerator<WaitForSeconds> turnBacktoWhite() {
        yield return new WaitForSeconds(5);
        problemImage.material = reset;
        problemImage.color = new Color(255,255,255);
        //loseTimer = false;
        stab = false;
    }

    public void resetPoints() {
        points = 0;
        pointsText.text = "Points: " + points.ToString();
    }

}
