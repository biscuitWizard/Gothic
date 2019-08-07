using FileHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpellParser
{
    class Program
    {
        public static string CurrentWorkingDirectory { get; private set; }

        [STAThread]
        static void Main(string[] args)
        {
            // build cwd
            var cwd = Environment.CurrentDirectory;
            var tokens = cwd.Split(Path.DirectorySeparatorChar);
            CurrentWorkingDirectory = string.Join(Path.DirectorySeparatorChar.ToString(), 
                tokens.Take(tokens.Length - 3)); // back three

            Console.WriteLine("Please select a sheet to parse:");
            var fd = new OpenFileDialog();
            fd.DefaultExt = ".csv";
            var fdResult = fd.ShowDialog();
            if(fdResult != DialogResult.OK)
            {
                Environment.Exit(-1);
            }

            Console.WriteLine("Parsing: {0}...", fd.FileName);
            var engine = new FileHelperEngine<RawSpell>();
            var results = engine.ReadFile(fd.FileName);
            var spells = new List<Spell>();
            foreach (var result in results.Skip(1))
            {
                if (string.IsNullOrEmpty(result.SpellName))
                    continue;
                var spell = new Spell
                {
                    SpellMechanics = result.SpellMechanics,
                    SpellName = result.SpellName,
                    Power = Int32.Parse(result.P),
                    Range = Int32.Parse(result.R),
                    Duration = Int32.Parse(result.D),
                    Element = result.Element,
                    Key = result.Element + "/" + result.Group,
                    Complexity = Int32.Parse(result.Complex),
                    Techniques = SortTechniques(ParseSplit(result.Techniques, ',')),
                    Adjuncts = ParseSplit(result.Adjuncts, ','),
                    Fallacies = ParseSplit(result.Fallacies, ','),
                    Domains = ParseSplit(result.Domains, '/'),
                    Type = result.Type.Trim(' ')
                };
                spells.Add(spell);
            }
            Console.WriteLine("Done.");

            Console.WriteLine("Loading arcane codex...");
            var codex = ArcaneCodex.Initialize();
            Console.WriteLine("Done.");

            Console.WriteLine("Loading fonts...");
            var gothicFonts = new PrivateFontCollection();
            gothicFonts.AddFontFile(Path.Combine(CurrentWorkingDirectory, @"AquilineTwo.ttf"));
            gothicFonts.AddFontFile(Path.Combine(CurrentWorkingDirectory, @"Of Wildflowers and Wings 2.ttf"));
            var aquilineTwoFont = new Font(gothicFonts.Families[0], 18f);
            var complexityFont = new Font(gothicFonts.Families[1], 36);
            var fallacyFont = new Font(gothicFonts.Families[1], 30, FontStyle.Bold);
            var mechanicsFont = new Font(gothicFonts.Families[1], 26);
            Console.WriteLine("Done.");

            Console.WriteLine("Generating spells...");

            var background = Bitmap.FromFile(Path.Combine(CurrentWorkingDirectory, 
                @"Layers\Background.png"));
            var source1 = Bitmap.FromFile(Path.Combine(CurrentWorkingDirectory, 
                @"Layers\Clear_Base_v2_-_No_Touchy.png")); // your source images - assuming they're the same size
            var aurumSource = Bitmap.FromFile(Path.Combine(CurrentWorkingDirectory,
                @"Layers\Aurum_Clear_Base_v2_-_No_Touchy.png")); // your source images - assuming they're the same size
            var source2 = Bitmap.FromFile(Path.Combine(CurrentWorkingDirectory, 
                @"Layers\Layer_121.png"));
            var source3 = Bitmap.FromFile(Path.Combine(CurrentWorkingDirectory,
                @"Layers\~What_is_written_here_can_only_be_understood_if_you_have_Magic.png"));
            var source4 = Bitmap.FromFile(Path.Combine(CurrentWorkingDirectory, 
                @"Layers\Thesis.png"));

            var outputDir = Path.Combine(CurrentWorkingDirectory, "Output");
            foreach (var spell in spells)
            {
                Image arrayBase;
                if (spell.Fallacies.Any(s => s.Contains("Aurum")))
                {
                    arrayBase = aurumSource;
                }
                else
                {
                    arrayBase = source1;
                }

                if (!Directory.Exists(Path.Combine(outputDir, spell.Key)))
                    Directory.CreateDirectory(Path.Combine(outputDir, spell.Key));
                var filename = Path.Combine(Path.Combine(outputDir, spell.Key), spell.SpellName + ".png");
                var target = new Bitmap(arrayBase.Width, arrayBase.Height, PixelFormat.Format32bppArgb);

                var graphics = Graphics.FromImage(target);
                graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear

                // ------------------------------------------
                // Ensure the best possible quality rendering
                // ------------------------------------------
                // The smoothing mode specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing). 
                // One exception is that path gradient brushes do not obey the smoothing mode. 
                // Areas filled using a PathGradientBrush are rendered the same way (aliased) regardless of the SmoothingMode property.
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // The interpolation mode determines how intermediate values between two endpoints are calculated.
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Use this property to specify either higher quality, slower rendering, or lower quality, faster rendering of the contents of this Graphics object.
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // This one is important
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                graphics.DrawImage(background, 0, 0);
                graphics.DrawImage(arrayBase, 0, 0);
                graphics.DrawImage(source2, 0, 0);
                graphics.DrawImage(source3, 0, 0);
                graphics.DrawImage(source4, 0, 0);

                graphics.DrawImage(codex.Elements[spell.Element.ToLower()], 0, 0);
                graphics.DrawImage(codex.Durations[spell.Duration.ToString()], 0, 0);
                graphics.DrawImage(codex.Powers[spell.Power.ToString()], 0, 0);
                graphics.DrawImage(codex.Ranges[spell.Range.ToString()], 0, 0);

                if (!codex.Types.ContainsKey(spell.Type.ToLower()))
                    throw new Exception("Could not find spell type: " + spell.Type);
                graphics.DrawImage(codex.Types[spell.Type.ToLower()], 0, 0);

                var domain = codex.Domains.Keys.FirstOrDefault(k => k.Contains(spell.Domains[0].ToLower()));
                if (domain == null)
                    throw new Exception("Couldn't find domain: " + spell.Domains[0]);
                graphics.DrawImage(codex.Domains[domain], 0, 0);

                if (spell.Domains.Length > 1)
                {
                    graphics.DrawImage(codex.SubdomainCircle1, 0, 0);

                    var subdomain = codex.Domains.Keys.FirstOrDefault(k => k.Contains(spell.Domains[1].ToLower()));
                    if (subdomain == null)
                        throw new Exception("Couldn't find domain: " + spell.Domains[1]);
                    graphics.DrawImage(codex.Subdomains1[subdomain], 0, 0);
                }

                if (spell.Domains.Length > 2)
                {
                    graphics.DrawImage(codex.SubdomainCircle2, 0, 0);

                    var subdomain = codex.Domains.Keys.FirstOrDefault(k => k.Contains(spell.Domains[2].ToLower()));
                    if (subdomain == null)
                        throw new Exception("Couldn't find domain: " + spell.Domains[2]);
                    graphics.DrawImage(codex.Subdomains2[subdomain], 0, 0);
                }

                if (spell.Domains.Length > 3)
                {
                    graphics.DrawImage(codex.SubdomainCircle3, 0, 0);

                    var subdomain = codex.Domains.Keys.FirstOrDefault(k => k.Contains(spell.Domains[3].ToLower()));
                    if (subdomain == null)
                        throw new Exception("Couldn't find domain: " + spell.Domains[3]);
                    graphics.DrawImage(codex.Subdomains3[subdomain], 0, 0);
                }

                if (spell.Techniques.Length > 0)
                {
                    var techniqueCount = new Dictionary<string, int>
                    {
                        { "Form", 0 },
                        { "Praxis", 0 },
                        { "Seal", 0 },
                        { "Theorem", 0 }
                    };
                    foreach (var techniqueName in spell.Techniques)
                    {
                        var cleansedName = techniqueName.Last(':').Replace("Form", "").Replace("Praxis", "")
                            .Trim(' ').ToLower().Replace(' ', '_');
                        if (techniqueName.Contains("Domain Specialization")) // domain spec hack
                            cleansedName = "domain_specialization";
                        var techniqueKey = codex.Techniques.Keys.FirstOrDefault(k => k.Contains(cleansedName));
                        if (techniqueKey == null)
                        {
                            if(techniqueName.ToLower().StartsWith("arch"))
                            {
                                Console.WriteLine("[WARNING] {0} for spell {1} should be in the adjuncts column.", techniqueName, spell.SpellName);
                                continue;
                            }
                            Console.WriteLine("[WARNING] Could not find technique " + techniqueName);
                            continue;
                        }

                        var technique = codex.Techniques[techniqueKey];
                        techniqueCount[technique.Type] += 1;

                        if (techniqueCount[technique.Type] < 2) {
                            foreach (var image in technique.Images)
                            {
                                graphics.DrawImage(image, 0, 0);
                            }
                        } else
                        {
                            switch(technique.Type)
                            {
                                case "Form":
                                    graphics.DrawImage(codex.ArchformCircle, 0, 0);
                                    break;
                                case "Theorem":
                                    graphics.DrawImage(codex.ArchtheoremCircle, 0, 0);
                                    break;
                                case "Praxis":
                                    graphics.DrawImage(codex.ArchpraxisCircle, 0, 0);
                                    break;
                                case "Seal":
                                    graphics.DrawImage(codex.ArchsealCircle, 0, 0);
                                    break;
                            }
                            var bonusTechnique = codex.BonusTechniques[techniqueKey];
                            foreach (var image in bonusTechnique.Images)
                            {
                                graphics.DrawImage(image, 0, 0);
                            }
                        }
                    }
                }

                // Create string formatting options (used for alignment)
                StringFormat format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                // Spell Name
                graphics.DrawString(spell.SpellName, aquilineTwoFont, Brushes.Black, new RectangleF(230, 43, 500, 35), format);
                // Mechanics Text
                var mechanicsText = new StringBuilder(spell.SpellMechanics);
                if (spell.Adjuncts.Length > 0)
                {
                    mechanicsText.AppendLine(Environment.NewLine);
                    mechanicsText.AppendLine("Adjuncts: " + string.Join(", ", spell.Adjuncts));
                }
                if(spell.Techniques.Length > 0)
                {
                    mechanicsText.AppendLine(Environment.NewLine);
                    mechanicsText.AppendLine("Techniques: " + string.Join(", ", spell.Techniques));
                }
                graphics.DrawString(mechanicsText.ToString(), mechanicsFont, Brushes.Black, new RectangleF(100, 1040, 750, 220), format);

                // Complexity
                graphics.DrawString(spell.Complexity.ToString(), complexityFont, Brushes.Black, new RectangleF(422, 556, 110, 60), format);

                if (spell.Fallacies.Length > 0)
                {
                    DrawCurvedText(graphics, string.Join(", ", spell.Fallacies.Take(2)),
                        new Point(480, 480), 427, 3.6f, fallacyFont, Brushes.Black);
                }

                if (spell.Fallacies.Length > 2)
                {
                    DrawCurvedText(graphics, string.Join(", ", spell.Fallacies.Skip(2).Take(2)),
                        new Point(480, 480), 427, 2.65f, fallacyFont, Brushes.Black);
                }

                if (spell.Fallacies.Contains("Abstract"))
                {
                    var random = new Random(spell.SpellName.GetHashCode());
                    graphics.DrawImage(codex.Abstract, 0, 0);
                    foreach (var abstractImageSet in codex.AbstractImages)
                    {
                        graphics.DrawImage(abstractImageSet[random.Next(0, abstractImageSet.Count - 1)], 0, 0);
                    }
                }

                target.Save(filename, ImageFormat.Png);
            }

            Console.WriteLine("Generated {0} spells.", spells.Count);
            Console.WriteLine("Parser complete. Press any key to exit.");
            Console.ReadKey();
        }

        private static string[] SortTechniques(IEnumerable<string> input)
        {
            var arches = input.Where(i => i.StartsWith("Arch"));
            var results = new List<string>();
            results.AddRange(input.Except(arches));
            results.AddRange(arches);
            return results.ToArray();
        }

        private static void DrawCurvedText(Graphics graphics, string text, Point centre, float distanceFromCentreToBaseOfText, float radiansToTextCentre, Font font, Brush brush)
        {
            // Circumference for use later
            var circleCircumference = (float)(Math.PI * 2 * distanceFromCentreToBaseOfText);

            // Get the width of each character
            var characterWidths = GetCharacterWidths(graphics, text, font).ToArray();

            // The overall height of the string
            var characterHeight = graphics.MeasureString(text, font).Height;

            var textLength = characterWidths.Sum();

            // The string length above is the arc length we'll use for rendering the string. Work out the starting angle required to 
            // centre the text across the radiansToTextCentre.
            float fractionOfCircumference = textLength / circleCircumference;

            float currentCharacterRadians = radiansToTextCentre + (float)(Math.PI * fractionOfCircumference);

            for (int characterIndex = 0; characterIndex < text.Length; characterIndex++)
            {
                char @char = text[characterIndex];

                // Polar to cartesian
                float x = (float)(distanceFromCentreToBaseOfText * Math.Sin(currentCharacterRadians));
                float y = -(float)(distanceFromCentreToBaseOfText * Math.Cos(currentCharacterRadians));

                using (GraphicsPath characterPath = new GraphicsPath())
                {
                    characterPath.AddString(@char.ToString(), font.FontFamily, (int)font.Style, font.Size, Point.Empty,
                                            StringFormat.GenericTypographic);

                    var pathBounds = characterPath.GetBounds();

                    // Transformation matrix to move the character to the correct location. 
                    // Note that all actions on the Matrix class are prepended, so we apply them in reverse.
                    var transform = new Matrix();

                    // Translate to the final position
                    transform.Translate(centre.X + x, centre.Y + y);

                    // Rotate the character
                    var rotationAngleDegrees = currentCharacterRadians * 180F / (float)Math.PI - 180F;
                    transform.Rotate(rotationAngleDegrees);

                    // Translate the character so the centre of its base is over the origin
                    transform.Translate(-pathBounds.Width / 2F, -characterHeight);

                    characterPath.Transform(transform);

                    // Draw the character
                    graphics.FillPath(brush, characterPath);
                }

                if (characterIndex != text.Length - 1)
                {
                    // Move "currentCharacterRadians" on to the next character
                    var distanceToNextChar = (characterWidths[characterIndex] + characterWidths[characterIndex + 1]) / 2F;
                    float charFractionOfCircumference = distanceToNextChar / circleCircumference;
                    currentCharacterRadians -= charFractionOfCircumference * (float)(2F * Math.PI);
                }
            }
        }

        private static IEnumerable<float> GetCharacterWidths(Graphics graphics, string text, Font font)
        {
            // The length of a space. Necessary because a space measured using StringFormat.GenericTypographic has no width.
            // We can't use StringFormat.GenericDefault for the characters themselves, as it adds unwanted spacing.
            var spaceLength = graphics.MeasureString(" ", font, Point.Empty, StringFormat.GenericDefault).Width;

            return text.Select(c => c == ' ' ? spaceLength : graphics.MeasureString(c.ToString(), font, Point.Empty, StringFormat.GenericTypographic).Width / 1.35f);
        }

        private static string[] ParseSplit(string input, char separator)
        {
            var output = new List<string>();
            foreach (var i in input.Split(separator))
            {
                if (string.IsNullOrEmpty(i))
                    continue;
                output.Add(i.Trim(' '));
            }
            return output.ToArray();
        }
    }
}
