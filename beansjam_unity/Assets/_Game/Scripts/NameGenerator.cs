using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NameGenerator {
	private System.Random random;
	private List<string> names = new List<string>();
	private string[] prefixes = new string[]{
		"alpha", "proxima",	
	};
	private string[] planets = new string[]{
		"schmerkur", "schmenus", "schmerde", "schmars", "schupiter", "schmaturn",
		"schmuranus", "schmeptun", "schmuto",
	};
	private string[] funnyWords = new string[]{
		"beer", "cheesecake", "bla",
	};
	private int i = 0;
	private int suffix = 1;

	public NameGenerator() {
		names.AddRange(planets);
		for (int i = 0; i < prefixes.Length; i++) {
			for (int j = 0; j < funnyWords.Length; j++) {
				names.Add(prefixes[i]+" "+funnyWords[j]);
			}
		}
		names = names.OrderBy(a => Random.value).ToList();
	}

	public string GenerateName() {
		var name = names[i];

		i++;
		if (i >= names.Count) {
			i = 0;
			suffix++;
		}

		return name.ToUpper()+(suffix > 1 ? " "+suffix : "");
	}
}
