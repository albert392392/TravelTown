using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameObjectStateManager : MonoBehaviour {
    public Vector3 position;
    public Quaternion rotation;
    public bool isActive;
    public Color objectColor;
    public Sprite objectSprite;

    public void SaveState() {
        print("SaveState SaveState SaveState");
        // ذخیره پوزیشن، چرخش، فعال بودن و رنگ آبجکت
        position = transform.position;
        rotation = transform.rotation;
        isActive = gameObject.activeSelf;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            objectColor = spriteRenderer.color;
            objectSprite = spriteRenderer.sprite;
        }
        // همچنین می‌توانید رنگ و اسپرایت مربوط به Image را نیز ذخیره کنید
        Image imageComponent = GetComponent<Image>();
        if (imageComponent != null) {
            objectColor = imageComponent.color;
            objectSprite = imageComponent.sprite;
        }

        // ذخیره‌سازی در PlayerPrefs (یا هر روش ذخیره‌سازی دلخواه دیگر)
        PlayerPrefs.SetString(name + "_Position", position.ToString());
        PlayerPrefs.SetString(name + "_Rotation", rotation.ToString());
        PlayerPrefs.SetInt(name + "_IsActive", isActive ? 1 : 0);
        PlayerPrefs.SetString(name + "_Color", objectColor.ToString());
        PlayerPrefs.SetString(name + "_Sprite", objectSprite.name); // ذخیره نام اسپرایت
        PlayerPrefs.Save();
    }

    public void LoadState() {
        print("LoadState LoadState LoadState");
        // بازیابی اطلاعات از PlayerPrefs
        if (PlayerPrefs.HasKey(name + "_Position")) {
            position = StringToVector3(PlayerPrefs.GetString(name + "_Position"));
            rotation = StringToQuaternion(PlayerPrefs.GetString(name + "_Rotation"));
            isActive = PlayerPrefs.GetInt(name + "_IsActive") == 1;
           // objectColor = StringToColor(PlayerPrefs.GetString(name + "_Color"));
            objectSprite = Resources.Load<Sprite>(PlayerPrefs.GetString(name + "_Sprite"));

            transform.position = position;
            transform.rotation = rotation;
            gameObject.SetActive(isActive);

            // اعمال رنگ و اسپرایت به SpriteRenderer
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                spriteRenderer.color = objectColor;
                spriteRenderer.sprite = objectSprite;
            }

            // یا اعمال برای Image Component
            Image imageComponent = GetComponent<Image>();
            if (imageComponent != null) {
                imageComponent.color = objectColor;
                imageComponent.sprite = objectSprite;
            }
        }
    }

    // Helper methods to convert strings back to Vector3, Quaternion, Color
    private Vector3 StringToVector3(string str) {
        string[] values = str.Trim(new char[] { '(', ')' }).Split(',');
        return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
    }

    private Quaternion StringToQuaternion(string str) {
        string[] values = str.Trim(new char[] { '(', ')' }).Split(',');
        return new Quaternion(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
    }

 /*   private Color StringToColor(string str) {
        string[] values = str.Trim(new char[] { '(', ')' }).Split(',');
        return new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
    }*/


    private void OnApplicationQuit() {
        SaveState();
    }
    private void OnApplicationPause(bool pause) {
        SaveState();
    }
}
