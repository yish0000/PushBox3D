using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EntryPoint : MonoBehaviour {

    protected Game m_game;

	// Use this for initialization
	void Start () {
        m_game = new Game();
        m_game.Init();
    }
	
	// Update is called once per frame
	void Update () {

        // Update the game object.
        if (m_game != null)
            m_game.Update();
	}

    void OnClickTest() {

    }
}
