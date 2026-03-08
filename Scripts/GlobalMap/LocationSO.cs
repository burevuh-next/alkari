using UnityEngine;
using AlkariEvolution.Managers;

namespace AlkariEvolution.GlobalMap
{
  /// <summary>
  /// Типы локаций на глобальной карте
  /// </summary>
  public enum LocationType
  {
    Station,        //Станция
    Asteroid,       //Астероид с ресурсами
    Anomaly,        //Аномалия
    TechBase,       //База техников
    Nest,           //Гнездо Дэвораров
    StartingPoint,  //Стартовая точка
    QuestLocation   //Сюжетная локация
  }
  /// <summary>
  /// Данные локации (ScriptableObject)
  /// </summary>
  [CreateAssetMenu(fileName = "NewLocation", menuName = "Alkari/Location")]
  public class LocationSO : ScriptableObject
  {
    [Header("Основная информация")]
    public string locationName = "Неизвестная локация";
    public LocationType locationType = LocationType.Station;

    [Header("Визуальное представление")]
    public Sprite iconSprite;               //Иконка на карте
    public Color iconColor = Color.white;

    [Header("Характеристики")]
    public int dufficultyLevel = 1;       //Уровень сложности 1-5
    public Vector2 position;              //Позиция на карте
    public bool isExplored = false;       //Была ли исследована
    public bool isActive = true;          //Доступна ли сейчас
    
    [Header("Ресурсы")]
    public int metalReward = 0;
    public int elementReward = 0;

    [Header("Сюжет (если это сюжетная локация)")]
    public string questId;                //Идентификатор сюжетной линии
    public bool isQuestLocation = false;

    [Header("Префаб для тактической карты")]
    public GameObject tacticalMapPrefab;  //Префаб для загрузки боя
  }
}