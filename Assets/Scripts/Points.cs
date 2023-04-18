using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    // Start is called before the first frame update
    public float threshold = 50;
    public int points;
    public Text pointsText;
    public Material pointsBar;
    public Image pointer;
    private Vector2 size;

    private float maxPoints = 100;
    private float minPoints = 0;

    // Start is called before the first frame update
    void Start()
    {
        Rect t = pointer.transform.parent.GetComponent<RectTransform>().rect;
        size = new Vector2(t.width, t.height);
    }

    void Update() {
        updatePointsBar();
    }

    public int getPoints() {
        return points;
    }
    public void updatePoints(int value) {
        points += value;
        // pointsText.text = "Points: " + points.ToString();
        updatePointsBar();
    }

    private bool loseTimer = false;
    public void losePoints(int value) {
        if (!loseTimer) {
            loseTimer = true;
            points -= value;
            // pointsText.text = "Points: " + points.ToString();
            updatePointsBar();
            
            StartCoroutine(allowHit());
        }
    }

    private bool stab = false;
    public void marcoCollision()
    {
        if (!stab)
        {
            stab = true;
            GetComponent<EventManager>().SendMarcoCollision();
            StartCoroutine(allowHit());
        }
    }

    IEnumerator<WaitForSeconds> allowHit() {
        yield return new WaitForSeconds(5);
        stab = false;
    }

    public void resetPoints() {
        points = 0;
        // pointsText.text = "Points: " + points.ToString();
        
        minPoints = 0;
        maxPoints = 100;
        updatePointsBar();
    }
    

    void updatePointsBar() {
        minPoints = Mathf.Min(minPoints, points);
        maxPoints = Mathf.Max(maxPoints, points);
        
        float p = 1.0f - ((points - minPoints) / (maxPoints - minPoints));
        // Debug.Log(p);
        pointsBar.SetFloat("_Points", p);
        var position = pointer.GetComponent<RectTransform>().localPosition;
        pointer.GetComponent<RectTransform>().localPosition = new Vector3(size.x * p - (size.x/2.0f), position.y,position.z);
    }

}
