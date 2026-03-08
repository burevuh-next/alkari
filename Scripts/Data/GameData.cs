using UnityEngine;
using System;
using System.Collections.Generic;


[System.Serializable]
public class GameData
{
    //Ресусы
    public int metal;
    public int element;
    public int organic;
    public int fuel;
    public int water;

    //Позиция на глобальной карте
    public float shipPositionX;
    public float shipPositionY;

    //Список выживших (позже расширим)
    public List<string> survivors = new List<string>();

    //Исследования (позже)
    public List<string> unlockedTech = new List<string>();

    //Дата и время сохранения
    public string saveDate;

    //Версия игры
    public string gameVersion = "0.1.0";

    //Конструктор
    public GameData()
  {
    //устанавливаем текущую дату при создании
    saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
  }
}
