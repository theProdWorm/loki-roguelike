using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Numerics;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Vector3 = UnityEngine.Vector3;

public class DamageNumbers : MonoBehaviour
{

    public GameObject parentObject;
    public TextMeshProUGUI textPrefab;
    //public Transform target;
    public Vector3 offset;
    private Camera cam;
    
    private CanvasGroup canvasGroup;
    private RectTransform uiRect;
    
    private static ObjectPool<TextMeshProUGUI> textPool;
    private static List<numberInfo> numbers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        textPool = new ObjectPool<TextMeshProUGUI>(
            createFunc: CreateText,
            actionOnGet: GetText,
            actionOnRelease: ReleaseText,
            actionOnDestroy: DestroyText
        );
        numbers = new List<numberInfo>();
        var text = textPool.Get();
        textPool.Release(text);
    }

    void Start()
    {
        
        cam = Camera.main;
        canvasGroup = GetComponent<CanvasGroup>();
        uiRect = GetComponent<RectTransform>();
    }
    
    #region  pool
    private TextMeshProUGUI CreateText()
    {
        TextMeshProUGUI obj = Instantiate(textPrefab, parentObject.transform);
        obj.name = "damageNumber";
        obj.gameObject.SetActive(false);
        return obj;
    }

    private void GetText(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true);
    }

    private void ReleaseText(TextMeshProUGUI text)
    {
        text.text = "";
        text.gameObject.SetActive(false);
    }

    private void DestroyText(TextMeshProUGUI text)
    {
        Destroy(text.gameObject);
    }
    #endregion
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    void LateUpdate()
    {
        if(numbers.Count == 0) return;
        for (int i = 0; i < numbers.Count; i++)
        {
            var number = numbers[i];
            if (number.timeLeft <= 0)
            {
                textPool.Release(number.text);
                numbers.Remove(number);
                continue;
            }
            number.timeLeft -= Time.deltaTime;
            
            Vector3 screenPos = cam.WorldToScreenPoint(number.target + offset);
            
            bool visible = screenPos.z > 0 &&
                           screenPos.x >= 0 && screenPos.x <= Screen.width &&
                           screenPos.y >= 0 && screenPos.y <= Screen.height;
            
            if (visible)
                number.text.rectTransform.position = screenPos;
            
            numbers[i] = number;
            
        }
    }

    public static void CreateDamageNumber(Transform position, int damage)
    {
        var text = textPool.Get();
        var info = new numberInfo
        {
            text = text,
            timeLeft = 1,
            target = position.position
        };
        info.text.text = damage.ToString();
        numbers.Add(info);
    }

    private struct numberInfo
    {
        public TextMeshProUGUI text;
        public float timeLeft;
        public Vector3 target;
    }
}
