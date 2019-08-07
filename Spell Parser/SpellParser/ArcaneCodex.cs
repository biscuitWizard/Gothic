using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SpellParser
{
    public class ArcaneCodex
    {
        public Dictionary<string, Image> Elements { get; set; }
        public Dictionary<string, Image> Durations { get; set; }
        public Dictionary<string, Image> Ranges { get; set; }
        public Dictionary<string, Image> Powers { get; set; }
        public Dictionary<string, Image> Domains { get; set; }
        public Dictionary<string, Image> Subdomains1 { get; set; }
        public Dictionary<string, Image> Subdomains2 { get; set; }
        public Dictionary<string, Image> Subdomains3 { get; set; }
        public Dictionary<string, Image> Types { get; set; }
        public Dictionary<string, Technique> Techniques { get; set; }
        public Dictionary<string, Technique> BonusTechniques { get; set; }

        public List<List<Image>> AbstractImages { get; set; }

        public Image SubdomainCircle1 { get; set; }
        public Image SubdomainCircle2 { get; set; }
        public Image SubdomainCircle3 { get; set; }

        public Image ArchformCircle { get; set; }
        public Image ArchtheoremCircle { get; set; }
        public Image ArchpraxisCircle { get; set; }
        public Image ArchsealCircle { get; set; }

        public Image Abstract { get; set; }

        public ArcaneCodex()
        {
            Elements = new Dictionary<string, Image>();
            Durations = new Dictionary<string, Image>();
            Ranges = new Dictionary<string, Image>();
            Powers = new Dictionary<string, Image>();
            Domains = new Dictionary<string, Image>();
            Subdomains1 = new Dictionary<string, Image>();
            Subdomains2 = new Dictionary<string, Image>();
            Subdomains3 = new Dictionary<string, Image>();
            Types = new Dictionary<string, Image>();
            Techniques = new Dictionary<string, Technique>();
            BonusTechniques = new Dictionary<string, Technique>();
            AbstractImages = new List<List<Image>>();
        }

        public static ArcaneCodex Initialize()
        {
            var codex = new ArcaneCodex();
            Console.WriteLine("Loading images from {0}...", Program.CurrentWorkingDirectory);

            codex.SubdomainCircle1 = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Sub-Domain_1_(Optional)\Group_1.png"));
            codex.SubdomainCircle2 = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Sub-Domain_2_(Optional)\Group_1_copy.png"));
            codex.SubdomainCircle3 = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Sub-Domain_3_(Optional)\Group_1_copy_2.png"));

            codex.ArchformCircle = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Fire_Bonus_Form\Base_copy.png"));
            codex.ArchpraxisCircle = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Air_Bonus_Praxis\Base_copy_3.png"));
            codex.ArchsealCircle = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Water_Bonus_Seal\Base.png"));
            codex.ArchtheoremCircle = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Earth_Bonus_Theorem\Base_copy_2.png"));

            codex.Abstract = Bitmap.FromFile(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Abstract\Base.png"));

            foreach (var elementFile in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Elements")))
                codex.Elements.Add(Path.GetFileNameWithoutExtension(elementFile).ToLower(), Bitmap.FromFile(elementFile));
            foreach (var durationFile in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Number_Symbols\Duration")))
                codex.Durations.Add(Path.GetFileNameWithoutExtension(durationFile), Bitmap.FromFile(durationFile));
            foreach (var powerFile in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Number_Symbols\Power")))
                codex.Powers.Add(Path.GetFileNameWithoutExtension(powerFile), Bitmap.FromFile(powerFile));
            foreach (var rangeFile in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Number_Symbols\Range")))
                codex.Ranges.Add(Path.GetFileNameWithoutExtension(rangeFile), Bitmap.FromFile(rangeFile));
            foreach (var domainFile in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Domain")))
                codex.Domains.Add(Path.GetFileNameWithoutExtension(domainFile).ToLower(), Bitmap.FromFile(domainFile));
            foreach (var subdomainFile1 in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Sub-Domain_1_(Optional)")))
                codex.Subdomains1.Add(Path.GetFileNameWithoutExtension(subdomainFile1).ToLower(), Bitmap.FromFile(subdomainFile1));
            foreach (var subdomainFile2 in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Sub-Domain_2_(Optional)")))
                codex.Subdomains2.Add(Path.GetFileNameWithoutExtension(subdomainFile2).ToLower(), Bitmap.FromFile(subdomainFile2));
            foreach (var subdomainFile3 in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Sub-Domain_3_(Optional)")))
                codex.Subdomains3.Add(Path.GetFileNameWithoutExtension(subdomainFile3).ToLower(), Bitmap.FromFile(subdomainFile3));
            foreach (var typeFile in Directory.GetFiles(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Type")))
                codex.Types.Add(Path.GetFileNameWithoutExtension(typeFile).ToLower(), Bitmap.FromFile(typeFile));

            // Technique loading
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Techniques\Forms")))
                codex.Techniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(), 
                    new Technique { Type = "Form", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Techniques\Praxes")))
                codex.Techniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(), 
                    new Technique { Type = "Praxis", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Techniques\Seals")))
                codex.Techniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(), 
                    new Technique { Type = "Seal", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Techniques\Theorems")))
                codex.Techniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(), 
                    new Technique { Type = "Theorem", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });

            // Bonus technique loading
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Earth_Bonus_Theorem")))
                codex.BonusTechniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(),
                    new Technique { Type = "Theorem", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Fire_Bonus_Form")))
                codex.BonusTechniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(),
                    new Technique { Type = "Form", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Air_Bonus_Praxis")))
                codex.BonusTechniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(),
                    new Technique { Type = "Praxis", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });
            foreach (var techniqueDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory,
                @"Layers\Spell_Building\Techniques\Water_Bonus_Seal")))
                codex.BonusTechniques.Add(techniqueDir.Split(Path.DirectorySeparatorChar).Last().ToLower(),
                    new Technique { Type = "Seal", Images = Directory.GetFiles(techniqueDir).Select(d => Bitmap.FromFile(d)).ToList() });

            foreach (var abstractImageDir in Directory.GetDirectories(Path.Combine(Program.CurrentWorkingDirectory, 
                @"Layers\Spell_Building\Abstract")))
                codex.AbstractImages.Add(Directory.GetFiles(abstractImageDir).Select(f => Bitmap.FromFile(f)).ToList());
            return codex;
        }
    }
}
