﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour {

    Vector3 desti;
    RaycastHit hit;
	GameObject prerendu;
    //GameObject prospect;
	bool initiatable;
	Color OKcolor;
	Color WRONGColor;
	Material[] materials;
	SphereCollider sphere;
	public bool placable;
	PlayerControl home;
	public bool NearGround;
	Vector3 temp;
	GameObject tempProspect;
	//Prerenducollision script2;
	float i;

    GameObject tourelle;
    GameObject pretourelle;
    int cost;
    public bool BuildConfirm;
    public List<KeyValuePair<string, int>> AvailableTurrets;
    public int curretTurret = 0;

    void Start () 
	{
		home = GetComponentInParent<PlayerControl> ();
        if (!home.isMine)
        {
            return;
        }
        placable = true;
		initiatable = true;
		OKcolor = new Color (0, 255, 0);
		WRONGColor = new Color (255, 0, 0);
		NearGround = true;
        
        BuildConfirm = false;
        AvailableTurrets = new List<KeyValuePair<string, int>>();

        AddTurret("Cannon",0);
        home.MyUI.upgrade.Unlock("Cannon");
        //Init
        tourelle = (GameObject)Resources.Load(AvailableTurrets[curretTurret].Key);
        pretourelle = (GameObject)Resources.Load(AvailableTurrets[curretTurret].Key.Split('_')[0] + "Preview");
        cost = AvailableTurrets[curretTurret].Value;
    }

    public void AddTurret(string turret, int level)
    {
        if (AvailableTurrets.Exists(x => x.Key.Contains(turret))) // Si la tourelle on la possède déja...
        {
            //var tu = AvailableTurrets.Exists(x => x.Key.Contains(turret));
            return;
        }
        switch (turret)
        {
            case "Cannon":
                AvailableTurrets.Add(new KeyValuePair<string, int>("Cannon_up"+level, 10));
                home.MyUI.AddTurret("Cannon", 10);
                return;
            case "Hammer":
                AvailableTurrets.Add(new KeyValuePair<string, int>("Hammer_up" + level, 20));
                home.MyUI.AddTurret("Hammer", 20);
                return;
            case "CrossBow":
                AvailableTurrets.Add(new KeyValuePair<string, int>("CrossBow_up" + level, 30));
                home.MyUI.AddTurret("CrossBow", 30);
                return;
            case "Cristal":
                AvailableTurrets.Add(new KeyValuePair<string, int>("Cristal_up" + level, 30));
                home.MyUI.AddTurret("Cristal", 30);
                return;
            case "Projector":
                AvailableTurrets.Add(new KeyValuePair<string, int>("Projector_up" + level, 30));
                home.MyUI.AddTurret("Projector", 30);
                return;
            case "Rocket":
                AvailableTurrets.Add(new KeyValuePair<string, int>("Rocket_up" + level, 30));
                home.MyUI.AddTurret("Rocket", 30);
                return;
            case "Regen":
                AvailableTurrets.Add(new KeyValuePair<string, int>("Regen_up" + level, 30));
                home.MyUI.AddTurret("Regen", 30);
                return;
            default:
                Debug.LogError("No turret nammed '" + turret + "'");
                return;
        }
    }

    public void UpgradeTurret(string name)
    {
        //Debug.Log(name);
        var turret = AvailableTurrets.Find(x => x.Key.Contains(name));
        var list = turret.Key.Split(new string[1] { "_up" }, System.StringSplitOptions.None);
        int newlvl = (int.Parse(list[1]) + 1);
        AvailableTurrets.Remove(turret);
        AvailableTurrets.Add(new KeyValuePair<string, int>(list[0] + "_up" + newlvl, turret.Value));
        //Debug.Log(list[0] + "_up" + newlvl);
        tourelle = (GameObject)Resources.Load(AvailableTurrets[curretTurret].Key);
        pretourelle = (GameObject)Resources.Load(AvailableTurrets[curretTurret].Key.Split('_')[0] + "Preview");
    }

    public int GetLevel(string name)
    {
        var turret = AvailableTurrets.Find(x => x.Key.Contains(name));
        var list = turret.Key.Split(new string[1] { "_up" }, System.StringSplitOptions.None);
        return int.Parse(list[1]);
    }

    void Update()
    {
        if (home.isMine == false && PhotonNetwork.connected == true || home.game.paused)
        {
            return;
        }
        if (BuildConfirm && ((Input.GetButtonDown("Cancel") && !home.useController) || (Input.GetButtonDown("2-Cancel") && home.useController)))
        {
            home.MyUI.UITurret.SetActive(false);
            BuildConfirm = false;
        }
        if (Input.GetButtonDown("Build") && !home.useController || (Input.GetButtonDown("2-Build") && home.useController))
        {
            if (BuildConfirm)
            {
                if (Confirm())
                {
                    home.MyUI.UITurret.SetActive(false);
                    BuildConfirm = false;
                }

            }
            else
            {
                home.MyUI.UITurret.SetActive(true);
                SetObjPropect(pretourelle);
                SetObj(tourelle);
                BuildConfirm = true;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && !home.useController || (Input.GetAxis("2-Mouse ScrollWheel") > 0 && home.useController))
        {
            if (BuildConfirm)
            {
                curretTurret = curretTurret - 1;
                if (curretTurret < 0)
                {
                    curretTurret = AvailableTurrets.Count - 1;
                }
                ChangeTurret(curretTurret);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && !home.useController || (Input.GetAxis("2-Mouse ScrollWheel") < 0 && home.useController))
        {
            if (BuildConfirm)
            {
                curretTurret = curretTurret + 1;
                if (curretTurret >= AvailableTurrets.Count)
                {
                    curretTurret = 0;
                }
                ChangeTurret(curretTurret);
            }
        }

        if (Input.GetAxis("Rotate") < 0 && !home.useController || (Input.GetAxis("2-Rotate") < 0 && home.useController))
        {
            if (BuildConfirm)
            {
                LeftRotate();
            }
        }
        if (Input.GetAxis("Rotate") > 0 && !home.useController || (Input.GetAxis("2-Rotate") > 0 && home.useController))
        {
            if (BuildConfirm)
            {
                RightRotate();
            }
        }
        if ((Input.GetButtonDown("Cancel") && !home.useController) || (Input.GetButtonDown("2-Cancel") && home.useController))
        {
            Cancel();
            home.MyUI.UITurret.SetActive(false);
            BuildConfirm = false;
        }


    }

    void LateUpdate () 
	{
		if (!initiatable) 
		{
			Cast ();
		}
    }

	public void SetObj(GameObject build)
	{
		this.prerendu = build;
	}
	public void SetObjPropect (GameObject prospect)
	{
		if (initiatable) 
		{
			//this.prospect = prospect;
			initiatable = false;
			if (true) 
			{
				tempProspect = Instantiate (prospect ,transform.position, new Quaternion(0,0,0,0),transform);
				//script2 = tempProspect.GetComponent<Prerenducollision> ();
				tempProspect.transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				i = tempProspect.transform.localRotation.y;

				//materials = prerendu.GetComponentsInChildren<Material>();
			} 
			else 
			{
				//initiatable = true;
				//eventuellement, message d'erreur par la suite
			}
		}
	}

    public void Cast()
    {
		
		if (placable && NearGround) 
		{
			foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()) 
			{
				rend.material.color = OKcolor;
			}
		}
		else 
		{
			foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()) 
			{
				rend.material.color = WRONGColor;
			}
		}
	}

	public bool Confirm ()
	{
        //Debug.Log(home.gold >= cost);
        //Debug.Log(home.gold + ", " + cost);
		if (placable && NearGround && home.gold >= cost)
		{
            // Pour que le master client soit le propriétaire, et pas que les tourelles dépop quand on se déco.
            home.view.RPC("PlaceTurret", PhotonTargets.MasterClient, prerendu.name, gameObject.transform.position, tempProspect.transform.rotation, home.view.viewID);
            home.gold -= cost;
            Destroy (tempProspect);
			initiatable = true;
            return true;
		} 
		else
		{
			//script.TurretBuildFailed (); Inutile.
            return false;
		}
	}

    public void ChangeTurret(string turretName)
    {
        Cancel();
        curretTurret = AvailableTurrets.FindIndex(x => x.Key.Contains(turretName));
        KeyValuePair<string, int> turret = AvailableTurrets[curretTurret];
        tourelle = (GameObject)Resources.Load(turret.Key);
        pretourelle = (GameObject)Resources.Load(turret.Key.Split('_')[0] + "Preview");
        cost = turret.Value;
        SetObjPropect(pretourelle);
        SetObj(tourelle);
    }

    public void ChangeTurret(int i)
    {
        KeyValuePair<string, int> turret = AvailableTurrets[i];
        curretTurret = i;
        Cancel();
        tourelle = (GameObject)Resources.Load(turret.Key);
        pretourelle = (GameObject)Resources.Load(turret.Key.Split('_')[0] + "Preview");
        cost = turret.Value;
        SetObjPropect(pretourelle);
        SetObj(tourelle);
    }

    public void TurretBuildFailed()
    {
        this.BuildConfirm = true;
    }

    public void IsPlacable(bool placable)
	{
		this.placable = placable;
	}
	public void IsGrounded(bool grounded)
	{
		this.NearGround = grounded;
	}
	public void RightRotate()
	{
		i += 5;
		tempProspect.transform.localRotation = Quaternion.Euler (new Vector3 (0, i, 0));
	}
	public void LeftRotate()
	{
		i -= 5;
		tempProspect.transform.localRotation = Quaternion.Euler (new Vector3 (0, i, 0));
	}
	public void Cancel()
	{
		if (!initiatable)
		{
		Destroy (tempProspect);
		initiatable = true;
		}
	}


}