using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EntryPoint : MonoBehaviour {

    static EntryPoint s_instance;

    public static EntryPoint Instance
    {
        get { return s_instance; }
    }

    protected Game m_game;

    void Awake()
    {
        s_instance = this;

    }

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
}
