using UnityEngine;
using System.Collections;
using System.Net;
#if UNITY_IOS
using System.Net.Mail;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
#endif

public static class EmailUtility
{
    public static string emailURL = "http://omni.meticulous-technology.com/vilifyemail.php";
    
	public static bool hasReportedError = false;
	
	public static IEnumerator SendEmailWithAttachment (string recipient, string sender, string subject, string body, byte[] attachedImage)
	{
		hasReportedError = false;
		
		Debug.Log ("Sending email to " + recipient + ", from: " + sender + ", image length: " + attachedImage.Length);
		
#if BUILTIN_SMTP && UNITY_IOS
		yield return 0;
		MemoryStream ms = new MemoryStream(attachedImage);
		
		
		
        MailMessage message = new MailMessage();

        message.To.Add(recipient);
        message.Subject = "Villain portrait";

		message.Attachments.Add(new Attachment(ms, "Villain portrait.png"));
        message.Body = body;
        message.From = new MailAddress(sender, "Bond Villain");

		SmtpClient smtp = new SmtpClient("mail.spymuseum.org", 587);
		smtp.Credentials = (ICredentialsByHost) new System.Net.NetworkCredential("villain", "SpyVill@in5");
        smtp.EnableSsl = true;
        smtp.UseDefaultCredentials = false;        
		ServicePointManager.ServerCertificateValidationCallback = 
                delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
                    { return true; };
        smtp.Send(message); 
#else
		WWWForm form = new WWWForm();
		form.AddBinaryData("screenshot", attachedImage, "screenshot.png", "image/png");
		form.AddField("recipient", recipient);
		form.AddField("from", sender);
		form.AddField("subject", WWW.UnEscapeURL(subject));
		form.AddField("body", WWW.UnEscapeURL(body));
		
		WWW w = new WWW(emailURL, form);
		
		while (!w.isDone)
		{
			
			yield return 0;
		}
		
		if (w.error != null)
		{
			Debug.LogWarning("Error: " + w.error);
			hasReportedError = true;
		}
		else
		{
			Debug.Log ("Success: " + w.text);
			hasReportedError = false;
		}
#endif
	}
	
	
	public static IEnumerator DisplayEmailSending (UIRoot activeGUI)
	{
		Transform gosRoot = activeGUI.transform.Find("Camera/Email Feedback");
		
		if (gosRoot != null)
		{
			gosRoot.gameObject.SetActive(true);
			Transform go = gosRoot.transform.Find ("Sending message...");
			Transform go2 = gosRoot.transform.Find("Message failure");
			
			if (go != null)
			{
				if (go2 != null)
				{
					go2.gameObject.SetActive(false);
				}
				
				go.gameObject.SetActive(true);
				
				yield return new WaitForSeconds(1.5f);
				
				go.gameObject.SetActive(false);
				
				float startTime = Time.time;
				if (go2 != null)
				{
					while ((Time.time - startTime) < 15 && !EmailUtility.hasReportedError)
					{
						yield return new WaitForSeconds(0.1f);
					}
					
					if (EmailUtility.hasReportedError)
					{
						go2.gameObject.SetActive(true);
						
						yield return new WaitForSeconds(1);
						
						go2.gameObject.SetActive(false);
					}
				}
			}
			
			gosRoot.gameObject.SetActive(false);
		}
		else
		{
			yield return 0;
		}
		
	}
	
}
