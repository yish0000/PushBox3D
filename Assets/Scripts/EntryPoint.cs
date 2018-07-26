using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EntryPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {

        PriorityQueue<int> queue = new PriorityQueue<int>();
        for (int i = 0; i < 50; i++)
            queue.Push(Random.Range(1, 1000));

        string str = "";
        while (queue.Count > 0)
            str += queue.Pop().ToString() + ",";
        Debug.Log(str);
    }
	
	// Update is called once per frame
	void Update () {

        // Process all the events.
        EventProcessQueue.Instance.Update();
	}

    void OnClickTest() {

    }
}
