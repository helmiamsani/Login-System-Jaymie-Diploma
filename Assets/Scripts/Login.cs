using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

#region Stuff for Sending Email
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
#endregion


public class Login : MonoBehaviour
{
    [Header("Login Panels")]
    public InputField username;
    public InputField email;
    public InputField password;

    [Header("Create Account Panels")]
    public InputField newUsername;
    public InputField newEmail;
    public InputField newPassword;

    [Header("Panels")]
    public GameObject incorrectPassword;
    public GameObject login;
    public GameObject createAccount;

    [Header("Send Email")]
    public InputField sndEmail;
    public InputField sndUsername;
    public GameObject incorrectEmail;

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
        StartCoroutine(CreateUser(newUsername.text, newEmail.text, newPassword.text));
    }

    IEnumerator LoginUser(string username, string email, string password)
    {
        string loginUserURL = "http://localhost/nsirpg/Login.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
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
            login.SetActive(false);
        }
        else if (webRequest.downloadHandler.text == "Incorrect Password")
        {
            incorrectPassword.SetActive(true);
            login.SetActive(false);
        }
    }

    public void LoginCurrentUser()
    {
        StartCoroutine(LoginUser(username.text, email.text, password.text));
    }

    IEnumerator CheckEmail(string email)
    {
        string checkEmailURL = "http://localhost/nsirpg/CheckEmail.php";
        WWWForm form = new WWWForm();
        form.AddField("email_Post", email);
        UnityWebRequest webRequest = UnityWebRequest.Post(checkEmailURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if (webRequest.downloadHandler.text == "User Not Found")
        {
            incorrectEmail.SetActive(true);
        }
        else
        {
        }
    }

    // Sending Email
    void SendEmail(string _email, string _username)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("sqlunityclasssydney@gmail.com");
        mail.To.Add(_email);
        mail.Subject = "NSRIPG Password Reset";
        mail.Body = "Hello " + _username + "\nReset using this code: " + "CODE";

        // Connects to Google
        SmtpClient smptServer = new SmtpClient("smtp.gmail.com");
        // Be able to send through ports
        smptServer.Port = 25;
        // Login to Google
        smptServer.Credentials = new NetworkCredential("sqlunityclasssydney@gmail.com", "sqlpassword") as ICredentialsByHost;
        smptServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        { return true; };
        // Send Message
        smptServer.Send(mail);
        Debug.Log("Sending Email");
    }

    public void SendingEmail()
    {
        SendEmail(sndEmail.text, sndUsername.text);
    }
}
