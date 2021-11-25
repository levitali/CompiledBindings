using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFTest.ViewModels;

#nullable enable

namespace WPFTest.Views;

partial class Page2 : Page
{
	~Page2()
	{ 
	}

	public string Prop1 => "Hello";

	public Task<string> Prop2 => Task.FromResult("Hello");

	public void OnClick1(object sender, EventArgs e)
	{ 
	}

	public void OnClick2()
	{
	}
}

