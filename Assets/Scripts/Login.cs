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
    #region Variables

    [Header("Login Panels")]
    public InputField username;
    public InputField password;
    public GameObject incorrectUsername;
    public GameObject incorrectPassword;

    [Header("Create Account Panels")]
    public InputField newUsername;
    public InputField newEmail;
    public InputField newPassword;
    public GameObject usernameExists;

    [Header("Forgot Password Panels")]
    public InputField currentUsername;
    public InputField passwordNew;
    public InputField confirmPassword;

    [Header("Panels")]
    public GameObject loggedInPanel;
    public GameObject login;
    public GameObject createAccount;

    [Header("Send Email")]
    public InputField sndEmail;
    public InputField sndUsername;
    public GameObject incorrectEmail;
    
    #endregion

    private void Start()
    {
        incorrectPassword.SetActive(false);
    }

    IEnumerator CreateUser(string username, string email, string password)
    {
        string createUserURL = "http://localhost/nsirpg/InsertUser.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(createUserURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if(webRequest.downloadHandler.text == "Username Already Exists")
        {
            usernameExists.SetActive(true);
        }

        else if (webRequest.downloadHandler.text == "Success")
        {
            loggedInPanel.SetActive(true);
            usernameExists.SetActive(false);
        }
    }

    public void CreateNewUser()
    {
        StartCoroutine(CreateUser(newUsername.text, newEmail.text, newPassword.text));
    }

    IEnumerator LoginUser(string username, string password)
    {
        string loginUserURL = "http://localhost/nsirpg/UserLogin.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(loginUserURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if (webRequest.downloadHandler.text == "Logged In")
        {
            loggedInPanel.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "User is Incorrect")
        {
            incorrectUsername.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "Incorrect Pasword")
        {
            incorrectPassword.SetActive(true);
        }
    }

    public void LoginCurrentUser()
    {
        StartCoroutine(LoginUser(username.text, password.text));
    }

    IEnumerator CheckEmail(string email, string username)
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
            SendEmail(email, username);
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
        StartCoroutine(CheckEmail(sndEmail.text, sndUsername.text));
    }

    IEnumerator changePassword(string username, string newPassword, string confirmPassword)
    {
        string changePasswordURL = "http://localhost/nsirpg/UpdatePassword.php";
        WWWForm form = new WWWForm();
        form.AddField("username_Post", username);
        form.AddField("password_Post", newPassword);
        UnityWebRequest webRequest = UnityWebRequest.Post(changePasswordURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if(webRequest.downloadHandler.text == "Password Changed")
        {
            loggedInPanel.SetActive(true);
        }
    }
}
