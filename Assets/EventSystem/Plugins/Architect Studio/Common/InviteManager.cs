using UnityEngine;

public class InviteManager : AS3DNet
{
    public void SendInvitations(string p_fromName, string p_fromEmail, string[] p_toEmails, string p_subject, string p_body)
    {
        WWWForm form = new WWWForm();
        form.AddField("from_name", p_fromName);
        form.AddField("from_email", p_fromEmail);
        form.AddField("body", p_body);
        form.AddField("subject", p_subject);

        Debug.Log(p_toEmails.Length);

        for (int i = 0; i < p_toEmails.Length; i++)
        {
            if (!string.IsNullOrEmpty(p_toEmails[i]))
            {
                form.AddField(string.Format("to_email{0}", i), p_toEmails[i]);

                Debug.Log(string.Format("to_email{0}", i) + " " + p_toEmails[i]);
            }
        }

        StartCoroutine(Post("SendMail", form, Results));
    }
}
