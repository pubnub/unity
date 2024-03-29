using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace PubnubApi.Unity.Internal.EditorTools {
	internal static class TemplateSetup {
		static readonly string template64 =
			@"dXNpbmcgU3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWM7CnVzaW5nIFVuaXR5RW5naW5lOwp1c2luZyBOZXd0b25zb2Z0Lkpzb247CnVzaW5nIFB1Ym51YkFwaTsKdXNpbmcgUHVibnViQXBpLlVuaXR5OwoKcHVibGljIGNsYXNzICNTQ1JJUFROQU1FIyA6IFBOTWFuYWdlckJlaGF2aW91ciB7CgkvLyBVc2VySWQgaWRlbnRpZmllcyB0aGlzIGNsaWVudC4KCXB1YmxpYyBzdHJpbmcgdXNlcklkOwojTk9UUklNIwoJcHJpdmF0ZSBhc3luYyB2b2lkIEF3YWtlKCkgewoJCWlmIChzdHJpbmcuSXNOdWxsT3JFbXB0eSh1c2VySWQpKSB7CgkJCS8vIEl0IGlzIHJlY29tbWVuZGVkIHRvIGNoYW5nZSB0aGUgVXNlcklkIHRvIGEgbWVhbmluZ2Z1bCB2YWx1ZSwgdG8gYmUgYWJsZSB0byBpZGVudGlmeSB0aGlzIGNsaWVudC4KCQkJdXNlcklkID0gU3lzdGVtLkd1aWQuTmV3R3VpZCgpLlRvU3RyaW5nKCk7CgkJfQoKCQkvLyBMaXN0ZW5lciBleGFtcGxlLgoJCWxpc3RlbmVyLm9uU3RhdHVzICs9IE9uUG5TdGF0dXM7CgkJbGlzdGVuZXIub25NZXNzYWdlICs9IE9uUG5NZXNzYWdlOwoJCWxpc3RlbmVyLm9uUHJlc2VuY2UgKz0gT25QblByZXNlbmNlOwoJCWxpc3RlbmVyLm9uRmlsZSArPSBPblBuRmlsZTsKCQlsaXN0ZW5lci5vbk9iamVjdCArPSBPblBuT2JqZWN0OwoJCWxpc3RlbmVyLm9uU2lnbmFsICs9IE9uUG5TaWduYWw7CgkJbGlzdGVuZXIub25NZXNzYWdlQWN0aW9uICs9IE9uUG5NZXNzYWdlQWN0aW9uOwoKCQkvLyBJbml0aWFsaXplIHdpbGwgY3JlYXRlIGEgUHViTnViIGluc3RhbmNlLCBwYXNzIHRoZSBjb25maWd1cmF0aW9uIG9iamVjdCwgYW5kIHByZXBhcmUgdGhlIGxpc3RlbmVyLiAKCQlJbml0aWFsaXplKHVzZXJJZCk7CgoJCS8vIFN1YnNjcmliZSBleGFtcGxlCgkJcHVibnViLlN1YnNjcmliZTxzdHJpbmc+KCkuQ2hhbm5lbHMobmV3W10geyAiVGVzdENoYW5uZWwiIH0pLkV4ZWN1dGUoKTsKCgkJLy8gUHVibGlzaCBleGFtcGxlCgkJYXdhaXQgcHVibnViLlB1Ymxpc2goKS5DaGFubmVsKCJUZXN0Q2hhbm5lbCIpLk1lc3NhZ2UoIkhlbGxvIFdvcmxkIGZyb20gVW5pdHkhIikuRXhlY3V0ZUFzeW5jKCk7Cgl9CgoJcHJpdmF0ZSB2b2lkIE9uUG5NZXNzYWdlQWN0aW9uKFB1Ym51YiBwbiwgUE5NZXNzYWdlQWN0aW9uRXZlbnRSZXN1bHQgcmVzdWx0KSB7CgkJRGVidWcuTG9nKHJlc3VsdC5DaGFubmVsKTsKCX0KCglwcml2YXRlIHZvaWQgT25QblNpZ25hbChQdWJudWIgcG4sIFBOU2lnbmFsUmVzdWx0PG9iamVjdD4gcmVzdWx0KSB7CgkJRGVidWcuTG9nKHJlc3VsdC5DaGFubmVsKTsKCX0KCglwcml2YXRlIHZvaWQgT25Qbk9iamVjdChQdWJudWIgcG4sIFBOT2JqZWN0RXZlbnRSZXN1bHQgcmVzdWx0KSB7CgkJRGVidWcuTG9nKHJlc3VsdC5DaGFubmVsKTsKCX0KCglwcml2YXRlIHZvaWQgT25QbkZpbGUoUHVibnViIHBuLCBQTkZpbGVFdmVudFJlc3VsdCByZXN1bHQpIHsKCQlEZWJ1Zy5Mb2cocmVzdWx0LkNoYW5uZWwpOwoJfQoKCXByaXZhdGUgdm9pZCBPblBuUHJlc2VuY2UoUHVibnViIHBuLCBQTlByZXNlbmNlRXZlbnRSZXN1bHQgcmVzdWx0KSB7CgkJRGVidWcuTG9nKHJlc3VsdC5FdmVudCk7Cgl9CgoJcHJpdmF0ZSB2b2lkIE9uUG5TdGF0dXMoUHVibnViIHBuLCBQTlN0YXR1cyBzdGF0dXMpIHsKCQlEZWJ1Zy5Mb2coc3RhdHVzLkNhdGVnb3J5ID09IFBOU3RhdHVzQ2F0ZWdvcnkuUE5Db25uZWN0ZWRDYXRlZ29yeSA/ICJDb25uZWN0ZWQiIDogIk5vdCBjb25uZWN0ZWQiKTsKCX0KCglwcml2YXRlIHZvaWQgT25Qbk1lc3NhZ2UoUHVibnViIHBuLCBQTk1lc3NhZ2VSZXN1bHQ8b2JqZWN0PiByZXN1bHQpIHsKCQlEZWJ1Zy5Mb2coJCJNZXNzYWdlIHJlY2VpdmVkOiB7cmVzdWx0Lk1lc3NhZ2V9Iik7Cgl9CgkgCglwcm90ZWN0ZWQgb3ZlcnJpZGUgdm9pZCBPbkRlc3Ryb3koKSB7CgkJLy8gVXNlIE9uRGVzdHJveSB0byBjbGVhbiB1cCwgZS5nLiB1bnN1YnNjcmliZSBmcm9tIGxpc3RlbmVycy4KCQlsaXN0ZW5lci5vblN0YXR1cyAtPSBPblBuU3RhdHVzOwoJCWxpc3RlbmVyLm9uTWVzc2FnZSAtPSBPblBuTWVzc2FnZTsKCQlsaXN0ZW5lci5vblByZXNlbmNlIC09IE9uUG5QcmVzZW5jZTsKCQlsaXN0ZW5lci5vbkZpbGUgLT0gT25QbkZpbGU7CgkJbGlzdGVuZXIub25PYmplY3QgLT0gT25Qbk9iamVjdDsKCQlsaXN0ZW5lci5vblNpZ25hbCAtPSBPblBuU2lnbmFsOwoJCWxpc3RlbmVyLm9uTWVzc2FnZUFjdGlvbiAtPSBPblBuTWVzc2FnZUFjdGlvbjsKCQkKCQliYXNlLk9uRGVzdHJveSgpOwoJfQp9";

		private const string templateFolder = "Assets/ScriptTemplates";

		private static readonly string templatePath =
			$"{templateFolder}/55-PubNub__PubNub Manager Script-PNManager.cs.txt";

		// [InitializeOnLoadMethod]
		public static void EnsureTemplate() {
			if (!File.Exists(templatePath)) {
				var data = Convert.FromBase64String(template64);
				Directory.CreateDirectory(templateFolder);
				File.WriteAllBytes(templatePath, data);

				// inform the user
				EditorUtility.DisplayDialog("PubNub",
					"Restart the Editor in order to have access to the script templates", "Ok");
			}
		}


		// Validated menu item.
		// Add a menu item named "Log Selected Transform Name" to MyMenu in the menu bar.
		// We use a second function to validate the menu item
		// so it will only be enabled if we have a transform selected.
		[MenuItem("PubNub/Set up templates")]
		static void SetUpTemplates()
		{
			EnsureTemplate();
		}

		// Validate the menu item defined by the function above.
		// The menu item will be disabled if this function returns false.
		[MenuItem("PubNub/Set up templates", true)]
		static bool ValidateSetUpTemplates()
		{
			// Return false if no transform is selected.
			return !File.Exists(templatePath);
		}
	}
}