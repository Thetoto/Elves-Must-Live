﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour {

    public Game game;
    public GameMode mode;
    
    //public float ennemyTime = 5;
    public bool wave = false;
	public int map;
    public float time;
    public Queue<KeyValuePair<string, float>> currentWave;
	public int ennemiesleft;
    public KeyValuePair<string, float> currentEnnemy = new KeyValuePair<string, float> (null, 0);
	GameObject SecondSpwn;
	Random rand;
	bool leaveroom;
    public bool endGame;
	float timerbeforeleaving;

    // Use this for initialization
    void Start () {
		timerbeforeleaving = 0f;
		leaveroom = false;
		rand = new Random();
		map = PlayerPrefs.GetInt ("Histoire");
		SecondSpwn = GameObject.Find("Spawn2");
        game = GetComponent<Game>();
		ennemiesleft = 0;
    }
	
    public bool StartWave()
    {
        if (!wave && mode.HasNextLevel())
        {
            currentWave = mode.LoadNextLevel();
			ennemiesleft = currentWave.Count;
            mode.level += 1;
        }
        else
        {
            return false;
        }
        LoadEnnemy();
        wave = true;
        return true;
    }

	// Update is called once per frame
	void Update () 
	{
        if (!PhotonNetwork.isMasterClient) 
		{
            return;
        }
		if (leaveroom) 
		{
			timerbeforeleaving += Time.deltaTime;
			if (timerbeforeleaving > 5) 
			{
				timerbeforeleaving = 0;
				Debug.Log ("lllllllllllleaving");
				PhotonNetwork.LeaveRoom();
			}
		}
        if (wave)
        {
            
            time -= Time.deltaTime;
            if (time <= 0)
            {
                //time = ennemyTime;
				if (map == 2) 
				{
					int temp = Random.Range (1, 3);
					if (temp == 1 || mode.level == 1) 
					{
						PhotonNetwork.InstantiateSceneObject (currentEnnemy.Key, gameObject.transform.position, Quaternion.identity, 0, new object[] { });
					}
					else 
					{
						PhotonNetwork.InstantiateSceneObject (currentEnnemy.Key, SecondSpwn.transform.position, Quaternion.identity, 0, new object[] { });
					}
				}
				else
				{
					PhotonNetwork.InstantiateSceneObject (currentEnnemy.Key, gameObject.transform.position, Quaternion.identity, 0, new object[] { });
				}
				ennemiesleft -= 1;

				if (ennemiesleft == 0)
                {
                    wave = false;
                    if (!mode.HasNextLevel())
                    {
                        endGame = true;
                        return;
                    }                    
                }
                else
                {
                    LoadEnnemy();
                }
            }
        }
        else
        {
            if (!game.ennemyInMap && !endGame)
            {
                game.masterClient.MyUI.Info.enabled = true;
                game.masterClient.MyUI.Info.text = Localization.Get("LaunchWave");
            }
        }

        if (endGame && !game.ennemyInMap)
        {
            game.masterClient.MyUI.Info.enabled = true;
            game.masterClient.MyUI.Info.text = Localization.Get("EndLevel");
            Finish();
        }
    }

    public void LoadEnnemy()
    {
        currentEnnemy = currentWave.Dequeue();
        time = currentEnnemy.Value;
    }

    public void Finish()
    {
        Debug.Log("You Win this level");
        if (PlayerPrefs.GetString("Mode") == "History")
        {
            PlayerPrefs.SetInt("Histoire", PlayerPrefs.GetInt("Histoire") + 1);
			//PhotonNetwork.LeaveRoom ();
			
        }
        leaveroom = true;
    }
}
