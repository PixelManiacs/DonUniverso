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

    /// <summary>
    /// Constructor
    /// </summary>
	public NameGenerator() {
        // Add the planet names to the names-List.
		names.AddRange(planets);

        // Combine the prefixes with the funny words.
		for (int i = 0; i < prefixes.Length; i++) {
			for (int j = 0; j < funnyWords.Length; j++) {
				names.Add(prefixes[i]+" "+funnyWords[j]);
			}
		}

        // sort list
		names = names.OrderBy(a => Random.value).ToList();
	}

    /// <summary>
    /// Generates the name for each planet.
    /// </summary>
    /// <returns></returns>
	public string GenerateName() {
		var name = names[i];
        
        // Increase the suffix when every name is taken.
		i++;
		if (i >= names.Count) {
			i = 0;
			suffix++;
		}

        // Return the name with suffix.
		return name.ToUpper()+(suffix > 1 ? " "+suffix : "");
	}
}
