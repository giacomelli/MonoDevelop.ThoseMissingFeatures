// 
// TransformAllTemplatesHandler.cs
// 
// Author:
//   Diego Giacomelli <giacomelli@gmail.com>
// 
// Copyright (C) 2013 Diego Giacomelli
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Mono.TextEditor;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.Ide;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;

namespace MonoDevelop.ThoseMissingFeatures.Commands
{
	/// <summary>
	/// Transform all templates handler.
	/// </summary>
	public class TransformAllTemplatesHandler : CommandHandler
	{
		/// <summary>
		/// Transforms all templates in all projects.
		/// </summary>
		protected override void Run ()
		{
			var statusBar = IdeApp.Workbench.StatusBar;
			var templates = GetCurrentProjectTemplates ();
		
			statusBar.ShowMessage ("Transforming all templates...");

			foreach (var t in templates) {
				CustomToolService.Update (t, true);
			}

			statusBar.ShowMessage (templates.Count + " templates transformed.");
		}

		/// <summary>
		/// Updates the infos about command.
		/// </summary>
		/// <param name="info">Info.</param>
		protected override void Update (CommandInfo info)
		{
			info.Visible = IdeApp.Workspace.IsOpen;

			if (info.Visible) {
				var templatesCount = GetCurrentProjectTemplates ().Count;

				if (templatesCount == 0) {
					info.Visible = false;
				} else {
					info.Text = String.Format (CultureInfo.InvariantCulture, "Tranform all templates ({0})", templatesCount);
				}
			}
		}

		/// <summary>
		/// Gets all templates.
		/// </summary>
		/// <returns>The all templates.</returns>
		private IList<ProjectFile> GetCurrentProjectTemplates ()
		{
			IList<ProjectFile> templates;

			var project = IdeApp.ProjectOperations.CurrentSelectedProject as DotNetProject;

			if (project == null) {
				templates = new List<ProjectFile> ();
			} else {
				templates = project.Files
				.Where
					(
					f => !String.IsNullOrEmpty (f.Generator)
					&& (
					    f.Generator.Equals ("TextTemplatingFileGenerator", StringComparison.OrdinalIgnoreCase)
					    || f.Generator.Equals ("TextTemplatingFilePreprocessor", StringComparison.OrdinalIgnoreCase)
					)
				)
				.ToList ();
			}

			return templates;
		}
	}
}