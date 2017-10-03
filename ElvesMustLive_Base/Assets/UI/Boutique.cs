﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boutique : MonoBehaviour {

    public bool TurretsSel = true;
    public bool WeaponsSel = false;

    public GameObject WeaponList;
    public GameObject TurretList;

    public UILabel selected;
    public UILabel description;
    public UILabel log;

    GameObject currentTurret = null;
    GameObject currentWeapon = null;

    public PlayerControl home;
    public UIControl ui;

    private void Start()
    {

    }

    public void OpenUpgrades()
    {
        ui.upgrade.gameObject.SetActive(true);
    }

    public void SelectTurret(GameObject obj)
    {
        currentTurret = obj;
        string str = obj.name;
        int price = int.Parse(obj.transform.GetChild(2).GetComponent<UILabel>().text.Split(' ')[0]);
        description.text = obj.transform.GetChild(5).GetComponent<UILabel>().text;
        selected.text = Localization.Get("cost") + " : " + price + " " + Localization.Get("gold_start"); 
    }
    public void SelectWeapon(GameObject obj)
    {
        currentWeapon = obj;
        string str = obj.name;
        int price = int.Parse(obj.transform.GetChild(2).GetComponent<UILabel>().text.Split(' ')[0]);
        description.text = Localization.Get(obj.name + "_des");
        selected.text = Localization.Get("cost") + " : " + price + " " + Localization.Get("gold_start");
    }

    public void BuySelected()
    {
        if (TurretsSel && currentTurret != null)
        {
            int price = int.Parse(currentTurret.transform.GetChild(2).GetComponent<UILabel>().text.Split(' ')[0]);
            string str = currentTurret.transform.name;
            if (price > home.gold)
            {
                log.text = Localization.Get("no_money");
                return;
            }
            else
            {
                home.gold -= price;
                log.text = Localization.Get(str) + Localization.Get("bought");
                description.text = Localization.Get("clickfordes");
            }
            home.raycast.AddTurret(currentTurret.transform.name, 0);
            ui.upgrade.Unlock(currentTurret.transform.name);
            TurretList.GetComponent<UIGrid>().RemoveChild(currentTurret.transform);
            Destroy(currentTurret);
            TurretList.GetComponent<UIGrid>().Reposition();
            currentTurret = null;
            selected.text = Localization.Get("select_a") + Localization.Get("turrets");
            ShowTurrets();
        }
        else if (WeaponsSel && currentWeapon != null)
        {
            int price = int.Parse(currentWeapon.transform.GetChild(2).GetComponent<UILabel>().text.Split(' ')[0]);
            string str = currentWeapon.transform.name;
            if (price > home.gold)
            {
                log.text = Localization.Get("no_money");
                return;
            }
            else
            {
                home.gold -= price;
                log.text = Localization.Get(str) + Localization.Get("bought");
                description.text = Localization.Get("clickfordes");
            }
            home.weapons.AddWeapon(currentWeapon.transform.name);
            WeaponList.GetComponent<UIGrid>().RemoveChild(currentWeapon.transform.GetSiblingIndex());
            Destroy(currentWeapon);
            WeaponList.GetComponent<UIGrid>().Reposition();
            currentWeapon = null;
            selected.text = Localization.Get("select_a") + Localization.Get("weapons");
            ShowWeapons();
            if (ui.upgrade.gameObject.GetActive())
            {
                ui.upgrade.SwitchToWeapons();
            } // Ca bug (plus) ...
        }
        else
        {
            log.text = Localization.Get("nothing_sel");
        }
    }

    public void ShowTurrets()
    {
        WeaponsSel = false;
        TurretsSel = true;
        WeaponList.SetActive(false);
        TurretList.SetActive(false);
        TurretList.SetActive(true);
        if (currentTurret != null)
        {
            SelectTurret(currentTurret);
        }
        else
        {
            selected.text = Localization.Get("select_a") + Localization.Get("turrets");
            description.text = Localization.Get("clickfordes");
        }
    }

    public void ShowWeapons()
    {
        WeaponsSel = true;
        TurretsSel = false;
        TurretList.SetActive(false);
        WeaponList.SetActive(false);
        WeaponList.SetActive(true);
        if (currentWeapon != null)
        {
            SelectWeapon(currentWeapon);
        }
        else
        {
            selected.text = Localization.Get("select_a") + Localization.Get("weapons");
            description.text = Localization.Get("clickfordes");
        }
    }
}
