using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListener : MonoBehaviour
{
    // We need:

    // The sequence to listen for has to be chars, for reasons
    private char[] sequence;
    private int indexListeningFor;
    private int sequenceLength;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecieveSequence(int[] seq)
    {
        indexListeningFor = 0;
        sequenceLength = seq.Length;
        char[] newSeq = new char[sequenceLength];
        for (int i = 0; i < sequenceLength; i++)
        {
            newSeq[i] = (char)seq[i];
        }
        sequence = newSeq;
        Debug.Log("Sequence received: " + sequence[0] + sequence[1] + sequence[2]);
    }
    private void OnGUI()
    {
        // TODO: make this next part only run for the big boi buttons
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.None)
        {
            Debug.Log("Detected character: " + e.character);
            //Debug.Log(Event.current.keyCode.ToString() + e.character);
            if (e.character == 'g')
            {
                GetComponent<EventManager>().GenerateSequence(3, 4);
            }
            else if (e.character == sequence[indexListeningFor])
            {
                if (indexListeningFor == sequenceLength - 1)
                {
                    GetComponent<EventManager>().SequenceCompleted();
                    Debug.Log("Sequence completed!!");
                }
                else
                {
                    indexListeningFor++;
                    Debug.Log("Sequence progressed!!");
                }
            }
            else
            {
                indexListeningFor = 0;
                Debug.Log("Wrong button pressed!! Progress reset.");
            }
        }
    }
}
