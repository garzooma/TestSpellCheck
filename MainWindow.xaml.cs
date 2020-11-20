using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestSpellCheck
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    internal static Uri spellUri;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      IList list = SpellCheck.GetCustomDictionaries(textBox);
      string fileName = "customwords.lex";
      System.IO.StreamReader file = new System.IO.StreamReader(fileName);
      string line;
      StringBuilder sb = new StringBuilder();
      while ((line = file.ReadLine()) != null)
      {
        System.Console.WriteLine(line);
        sb.Append(line);
        sb.Append("\r\n");
      }


      UTF8Encoding encoding = new UTF8Encoding();
      var bytes = encoding.GetBytes(sb.ToString());

      // Create in-memory Package to store the byte array
      MemoryStream stream = new MemoryStream();
      Package pack = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
      Uri packUri = new Uri("SpellCheckDictionary:");
      PackageStore.RemovePackage(packUri);
      PackageStore.AddPackage(packUri, pack);
      Uri packPartUri = new Uri("/Content", UriKind.Relative);
      PackagePart packPart = pack.CreatePart(packPartUri, "text/plain");
      if (packPart != null)
      {
        Stream packageStream = packPart.GetStream();
        packageStream.Write(bytes, 0, bytes.Length);
        packageStream.Close();
        spellUri = PackUriHelper.Create(packUri, packPart.Uri);
      }

      textBox.SpellCheck.CustomDictionaries.Clear();
      textBox.SpellCheck.CustomDictionaries.Add(spellUri);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      textBox.SpellCheck.CustomDictionaries.Clear();
    }
  }
}
