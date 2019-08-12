using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    
    public InputField username;
    public InputField email;
    public InputField password;
    public GameObject incorrectPassword;
    public GameObject signin;


    private void Start()
    {
        incorrectPassword.SetActive(false);
    }

    IEnumerator CreateUser(string username, string email, string password)
    {
        string createUserURL = "http://localhost/nsirpg/insertuser.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(createUserURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest);
    }

    public void CreateNewUser()
    {
        StartCoroutine(CreateUser(username.text, email.text, password.text));
    }

    IEnumerator LoginUser(string username, string password)
    {
        string loginUserURL = "http://localhost/nsirpg/Login.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(loginUserURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if (webRequest.downloadHandler.text == "Login Successful")
        {
            SceneManager.LoadScene(1);
        }
        else if (webRequest.downloadHandler.text == "Incorrect Username")
        {
            incorrectPassword.SetActive(true);
            signin.SetActive(false);
        }
        else if (webRequest.downloadHandler.text == "Incorrect Password")
        {
            incorrectPassword.SetActive(true);
            signin.SetActive(false);
        }
    }

    public void LoginCurrentUser()
    {
        StartCoroutine(LoginUser(username.text, password.text));
    }
}
