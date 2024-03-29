﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



#region Emailstuff
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
#endregion
public class SystemManager : MonoBehaviour
{
   
    private string user;
    private string password;
    private string email;
   
    public string serverText;
    [Header("Panels")]
    public GameObject firstPanel;
    public GameObject loginPanel;
    public GameObject forgotPasswordPannel;
    public GameObject forgotpasswordemail;
    public GameObject emailCodePanel;
    public GameObject newPasswordPanel;
    public GameObject createUserPanel;
    [Header("Create Account")]
    public InputField createUsername;
    public InputField createEmail;
    public InputField createPassword;
    [Header("Login")]
    public InputField loginUsername;
    public InputField loginPassword;
    [Header("Reset User")]
    public InputField resetPassword;
    public InputField resetEmail;
    public InputField resetUsername;

    [Header("Misc Texts")]
    public LoginErrors[] login;
    [Serializable]
    public class LoginErrors
    {
        public Text usernameLgnTexts;
        public Text passwordLgnTexts;
    }

    public CreateErrors[] createAccount;
    [Serializable]
    public class CreateErrors
    {
        public Text usernameCrtTexts;
        public Text EmailCrtTexts;
        public Text EmailWarning2;
    }

    public ResetPassword[] resetPasswords;
    [Serializable]
    public class ResetPassword
    {
        public Text mainWarning;
        public Text emailCode;
    }


    public void Start()
    {
        firstPanel.SetActive(true);
        loginPanel.SetActive(false);
        forgotPasswordPannel.SetActive(false);
        forgotpasswordemail.SetActive(false);
        emailCodePanel.SetActive(false);
        newPasswordPanel.SetActive(false);
        createUserPanel.SetActive(false);
    }

    #region Create
    IEnumerator CreateUser(string username, string password, string email)
    {
        string createUserURL = "http://localhost/nsripg/insertuser.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("email", email);
        UnityWebRequest webRequest = UnityWebRequest.Post(createUserURL, form);
        yield return webRequest.SendWebRequest();
       // serverText = webRequest.downloadHandler.text;
        Debug.Log(webRequest.downloadHandler.text);
        if(webRequest.downloadHandler.text == "Username Already Exists")
        {
            createAccount[0].usernameCrtTexts.enabled = true;
            createAccount[0].usernameCrtTexts.text = webRequest.downloadHandler.text;
        }
        else if(webRequest.downloadHandler.text == "Email Already Exists")
        {
            createAccount[0].EmailWarning2.enabled = false;
            createAccount[0].EmailCrtTexts.enabled = true;
            createAccount[0].EmailCrtTexts.text = webRequest.downloadHandler.text;
        }
        else if(webRequest.downloadHandler.text == "Email is not exist")
        {
            createAccount[0].EmailCrtTexts.enabled = false;
            createAccount[0].EmailWarning2.enabled = true;
            createAccount[0].EmailWarning2.text = webRequest.downloadHandler.text;
        }
    }

    public void CreateNewUser()
    {
        StartCoroutine(CreateUser(createUsername.text, createPassword.text, createEmail.text));
        user = createUsername.text;
        password = createPassword.text;
        email = createEmail.text;
    }
    #endregion

    #region Login
    IEnumerator LoginUser(string username, string password)
    {
        string loginUserURL = "http://localhost/nsripg/Login.php";
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
        else if(webRequest.downloadHandler.text == "Incorrect Password")
        {
            login[0].passwordLgnTexts.enabled = true;
            login[0].passwordLgnTexts.text = webRequest.downloadHandler.text;
        }
        else if(webRequest.downloadHandler.text == "Incorrect Username")
        {
            login[0].usernameLgnTexts.enabled = true;
            login[0].usernameLgnTexts.text = webRequest.downloadHandler.text;
        }
    }
    public void LoginUser()
    {
        StartCoroutine(LoginUser(loginUsername.text, loginPassword.text));
        
    }
    #endregion

    #region ForgotPasscode
    IEnumerator ForgotPassword( InputField email)
    {
        string ForgotURL = "http://localhost/nsripg/Email.php";
       
        WWWForm form = new WWWForm();
        form.AddField("email_Post", email.text);
        UnityWebRequest webRequest = UnityWebRequest.Post(ForgotURL, form);
       
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if (webRequest.downloadHandler.text == "User cannot be found")
        {
            resetPasswords[0].mainWarning.enabled = true;
            resetPasswords[0].mainWarning.text = webRequest.downloadHandler.text;
        }
        else
        {
            resetPasswords[0].mainWarning.enabled = false;
            user = webRequest.downloadHandler.text;
            SendEmail(email);
            forgotpasswordemail.SetActive(false);
            emailCodePanel.SetActive(true);
            resetPasswords[0].emailCode.enabled = true;
            resetPasswords[0].emailCode.text = "Code is sent to your email, " + resetEmail.text + ".";
        }
    }
    public void ForgotPassword()
    {
        StartCoroutine(ForgotPassword(resetEmail));
    }
    #endregion

    #region Email
    public void SendEmail(InputField email)
    {
       
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("sqlunityclasssydney@gmail.com");
        mail.To.Add(email.text);
        mail.Subject = "nsripg Password Reset";
        mail.Body = "Hello Welcome to Game Account reset page " + user + "\nReset using this code: " + code;

        //connect to google
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        //ne able to send through ports
        smtpServer.Port = 25; //numbors
        //login to google
        smtpServer.Credentials = new NetworkCredential("sqlunityclasssydney@gmail.com", "sqlpassword") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        { return true; };
        //send message
        smtpServer.Send(mail);
        Debug.Log("Sending Email");

    }
    #endregion
    public void CorrectCode(InputField codefield)
    {
        if (codefield.text == code)
        {
            Debug.Log("yes this is correct");
            emailCodePanel.SetActive(false);
            newPasswordPanel.SetActive(true);
        }
        else { Debug.Log("no this is not correct"); }
       //either make button that resends email or send them back to previous panel
    }
    
   IEnumerator ConfirmPassword(InputField newPassword1, InputField username)
    {
        string confirmPasswordURL = "http://localhost/nsripg/updatepassword.php";
        WWWForm form = new WWWForm();

        form.AddField("password_Post", newPassword1.text);
        form.AddField("username_Post", username.text);
        UnityWebRequest webRequest = UnityWebRequest.Post(confirmPasswordURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if (newPassword1.text == "")
        {
            Debug.Log("Please Enter your new password the field");
        }
        if (newPassword1.text != "")
        {
            Debug.Log("gongrats you made your new password");
            password = newPassword1.text;
           
            newPasswordPanel.SetActive(false);
            forgotPasswordPannel.SetActive(false);
            forgotpasswordemail.SetActive(true);
            loginPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Error");
        }
    }
    public void ConfirmNewPassword( )
    {
        StartCoroutine(ConfirmPassword(resetPassword, resetUsername));
    }



    #region generators
    #region mydumbgenerator


    //public string[] lettersLowerCase = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    //public string[] lettersUpperCase = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    //public int letterNum;
    //public int numbers;
    //public int chance;
    //public string fullCode;
    //public void CodeGen()
    //{
    //    fullCode = "";
    //    for (int i = 0; i < 26; i++)
    //    {
    //        chance = UnityEngine.Random.Range(0, 2);
    //        numbers = UnityEngine.Random.Range(0, 10);
    //        letterNum = UnityEngine.Random.Range(0, 27);
    //        fullCode += numbers;         
    //    }
    //    Debug.Log(fullCode);
    //}
    #endregion
    #region jamieusingbraingenerator
    
    private string characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*";
    private string code = "";
    public void CreateCode()
    {
        code = "";
        for (int i = 0; i < 10; i++)
        {
            int a = UnityEngine.Random.Range(0, characters.Length);
            code = code + characters[a];
        }
        Debug.Log(code);
    }
    #endregion
    #endregion


}
