namespace HeritageMarket.Infrastructure.Persistence.Seed;

public record SeedProduct(string Name, string Description, decimal Price, string? Gender = null, string? Sizes = null, string? ImageUrl = null);
public record SeedBook(string Title, string Author, string Description, decimal Price, string? ImageUrl = null);

public record SeedCountry(
    string Name, string Code, string Region, string Description,
    SeedProduct[] Home, SeedProduct[] Accessories, SeedProduct[] PhoneCovers, SeedProduct[] Wear,
    SeedBook[] Books);

/// <summary>
/// The full heritage catalog: countries grouped by region, each with multiple products in every
/// category. Book picks are real, well-known works (several classics) by authors celebrated both
/// in their home country and internationally.
/// </summary>
public static class HeritageCatalogSeedData
{
    public const string ArabWorld = "Arab World";
    public const string Asia = "Asia";
    public const string Europe = "Europe";
    public const string Americas = "Americas";

    public static readonly SeedCountry[] Countries =
    {
        new("Lebanon", "LB", ArabWorld, "Levantine heritage: cedar motifs, mosaic art, and mountain craftsmanship.",
            Home: new[]
            {
                new SeedProduct("Cedar-Carved Wooden Box", "Hand-carved Lebanese cedar wood box with traditional inlay patterns.", 45.00m),
                new SeedProduct("Beiruti Mosaic Tray", "Hand-set mosaic serving tray in a traditional Levantine geometric pattern.", 52.00m),
                new SeedProduct("Hand-Painted Cedar Jar", "Blue-and-white ceramic jar hand-painted with Lebanon's cedar tree emblem, signed by the artist.", 95.00m, ImageUrl: "/images/curated/home-lebanon-jug.jpg"),
                new SeedProduct("Embroidered Velvet Cushion Trio", "Set of three richly embroidered velvet cushions in traditional Levantine floral motifs.", 85.00m, ImageUrl: "/images/curated/lebanon-home-cushions.jpg"),
                new SeedProduct("Suzani-Embroidered Bench", "Vibrant hand-embroidered bench upholstered in traditional suzani-style needlework.", 320.00m, ImageUrl: "/images/curated/lebanon-home-suzani-bench.jpg"),
                new SeedProduct("Carved Wood Majlis Bench", "Hand-carved lattice-back wooden bench, traditional Lebanese mountain craftsmanship.", 260.00m, ImageUrl: "/images/curated/lebanon-home-carved-bench.png"),
                new SeedProduct("Mountain Village Oil Painting", "Original oil painting of a traditional Lebanese mountain village home, hand-signed by the artist.", 180.00m, ImageUrl: "/images/curated/lebanon-home-village-painting.jpg")
            },
            Accessories: new[]
            {
                new SeedProduct("Cedar Pendant Necklace", "Gold-plated pendant shaped after Lebanon's national cedar emblem.", 52.00m),
                new SeedProduct("Handwoven Raffia Tote", "A market tote handwoven in the coastal villages of Mount Lebanon.", 58.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Cedar Silhouette Phone Case", "Phone case printed with Lebanon's iconic cedar tree silhouette.", 22.00m),
                new SeedProduct("Beirut Mosaic Phone Case", "Phone case featuring a Levantine mosaic-tile pattern.", 24.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Embroidered Tatreez Thobe", "Traditional Levantine thobe with hand-stitched tatreez embroidery.", 120.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Mountain Village Kaftan", "Flowing kaftan inspired by traditional dress from Mount Lebanon.", 98.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Little Cedars Embroidered Vest", "A child-sized vest finished with a miniature tatreez embroidery pattern.", 48.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("The Prophet", "Khalil Gibran", "Gibran's 1923 collection of poetic essays, one of the most translated books ever written, blending Mount Lebanon imagery with mysticism.", 22.00m, "/images/curated/book-lebanon-prophet.webp"),
                new SeedBook("Leo Africanus", "Amin Maalouf", "A historical novel following a 16th-century Muslim-Andalusian diplomat, by one of Lebanon's most celebrated living writers.", 24.00m, "/images/curated/book-lebanon-baalbek.webp")
            }),

        new("Egypt", "EG", ArabWorld, "Ancient Nile heritage: papyrus art, pharaonic motifs, and Nubian craft.",
            Home: new[]
            {
                new SeedProduct("Hand-Painted Papyrus Art", "Framed hand-painted papyrus depicting motifs from ancient Egyptian wall art.", 52.00m),
                new SeedProduct("Nubian Woven Basket", "Hand-woven storage basket in traditional Nubian palm-leaf technique.", 38.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Ankh Symbol Bracelet", "Brass bracelet featuring the ankh, the ancient Egyptian symbol of life.", 28.00m),
                new SeedProduct("Khan el-Khalili Scarf", "Cotton scarf printed with motifs inspired by Cairo's historic bazaar.", 34.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Ankh Hieroglyph Phone Case", "Phone case printed with the ankh symbol and a hieroglyphic border.", 27.00m),
                new SeedProduct("Pyramids at Giza Phone Case", "Phone case featuring the Giza plateau at golden hour.", 25.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Egyptian Cotton Galabeya", "Loose-fitting galabeya woven from Egyptian cotton with tally metallic embroidery.", 76.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Nubian Embroidered Vest", "Vest finished with traditional Nubian geometric embroidery.", 64.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Mini Galabeya for Kids", "A child-sized galabeya woven from soft Egyptian cotton.", 42.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Palace Walk", "Naguib Mahfouz", "First novel of the Cairo Trilogy, by Egypt's Nobel laureate and the only Arabic writer to win the prize.", 25.00m, "/images/curated/book-egypt-treasures.jpg"),
                new SeedBook("The Days", "Taha Hussein", "A classic autobiography by 'the Dean of Arabic Literature,' recounting his Nile Delta childhood.", 21.00m, "/images/curated/book-egypt-breasted.jpg")
            }),

        new("Morocco", "MA", ArabWorld, "North African heritage: zellige tilework, Berber weaving, and desert leathercraft.",
            Home: new[]
            {
                new SeedProduct("Hand-Painted Blue Zellige Tagine", "Ceramic tagine hand-painted with indigo Amazigh star and lattice motifs, food-safe glaze.", 85.00m, ImageUrl: "/images/curated/morocco-tagine.webp"),
                new SeedProduct("Marrakech Doorways Wall Art Trio", "Set of three framed prints capturing ornate Moroccan and Rajasthani carved doorways.", 95.00m, ImageUrl: "/images/curated/morocco-doorways-wall-art.webp"),
                new SeedProduct("Indigo Quatrefoil Canvas Set", "Four-panel watercolor-style canvas set in a classic Moroccan quatrefoil lattice pattern.", 110.00m, ImageUrl: "/images/curated/morocco-quatrefoil-wall-art.webp")
            },
            Accessories: new[]
            {
                new SeedProduct("Amazigh Coral Coin Necklace", "Antique-style silver necklace with coral drops and engraved coin pendants, Amazigh tradition.", 92.00m, ImageUrl: "/images/curated/morocco-coral-necklace.webp"),
                new SeedProduct("Antique Silver Filigree Cuff", "Wide hand-hammered silver cuff with granulated filigree rosettes, Berber craftsmanship.", 78.00m, ImageUrl: "/images/curated/morocco-silver-cuff.jpg")
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Marrakech Star Mosaic Phone Case", "Bold multicolor geometric star mosaic phone case inspired by Fez zellige tilework.", 26.00m, ImageUrl: "/images/curated/morocco-star-mosaic-phonecase.jpg"),
                new SeedProduct("Bou Inania Tile Phone Case", "Phone case wrapped in an intricate turquoise-and-gold Moroccan tile medallion pattern.", 28.00m, ImageUrl: "/images/curated/morocco-tile-phonecase.jpg")
            },
            Wear: new[]
            {
                new SeedProduct("Ruby Embroidered Kaftan", "Deep-red kaftan with dense gold sequin and bead embroidery, cinched with a metallic sash.", 168.00m, "Women", "S,M,L,XL", "/images/curated/morocco-red-kaftan.jpg"),
                new SeedProduct("Emerald Velvet Takchita", "Forest-green velvet takchita with elaborate gold sfifa braid and a bejeweled belt.", 245.00m, "Women", "S,M,L,XL", "/images/curated/morocco-green-velvet-kaftan.jpg"),
                new SeedProduct("Hooded Djellaba with Cloak", "Cream wool djellaba layered under a flowing hooded black cloak, Marrakech tailoring.", 138.00m, "Men", "S,M,L,XL,XXL", "/images/curated/morocco-men-djellaba-cloak.jpg"),
                new SeedProduct("Velvet Tarbosh with Zellige Band", "Velvet tarbosh cap wrapped in a laser-cut metallic band echoing zellige star patterns.", 38.00m, "Men", "One Size", "/images/curated/morocco-tarbosh.webp"),
                new SeedProduct("Classic Cream Hooded Djellaba", "Traditional cream wool djellaba with a deep hood and hand-stitched trim, Marrakech tailoring.", 98.00m, "Men", "S,M,L,XL,XXL", "/images/curated/morocco-men-djellaba-hood.jpg"),
                new SeedProduct("Kids' Embroidered Green Djellaba", "Vibrant emerald djellaba with colorful floral hand-embroidery, sized for little ones.", 58.00m, "Kids", "4T,6,8,10,12", "/images/curated/morocco-kids-djellaba.webp")
            },
            Books: new[]
            {
                new SeedBook("Morocco: Its People and Places", "Edmondo De Amicis", "A two-volume 19th-century travel classic chronicling Morocco's cities, souks, and courts.", 58.00m, "/images/curated/morocco-book-peopleplaces.jpg"),
                new SeedBook("Once a Week", "H. A. Guerber", "An antique hand-tooled leather-bound volume of collected weekly tales and travel writing.", 34.00m, "/images/curated/morocco-book-onceaweek.jpg")
            }),

        new("Palestine", "PS", ArabWorld, "Levantine heritage rooted in olive groves, embroidery, and craft traditions passed through generations.",
            Home: new[]
            {
                new SeedProduct("Olive Wood Carved Bowl", "Hand-carved bowl from centuries-old Palestinian olive wood.", 42.00m),
                new SeedProduct("Hebron Blown Glass Vase", "Hand-blown glass vase from the historic glassblowing workshops of Hebron.", 56.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Tatreez Embroidery Bag", "Canvas tote hand-stitched with tatreez cross-stitch motifs unique to the region.", 74.00m),
                new SeedProduct("Olive Wood Rosary Beads", "Prayer beads hand-turned from Bethlehem olive wood.", 26.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Tatreez Pattern Phone Case", "Phone case printed with a traditional tatreez cypress-tree motif.", 25.00m),
                new SeedProduct("Olive Branch Phone Case", "Phone case featuring a hand-illustrated olive branch, a symbol of Palestinian land.", 23.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Palestinian Embroidered Thobe", "Full-length thobe hand-embroidered with tatreez cross-stitch across the chest panel.", 135.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Tatreez Trim Vest", "Cotton vest finished with a hand-embroidered tatreez border.", 72.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kids' Tatreez Dress", "A child-sized dress finished with a simplified tatreez cross-stitch border.", 52.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Memory for Forgetfulness", "Mahmoud Darwish", "A lyrical memoir by the Palestinian national poet, a landmark of modern Arabic literature.", 24.00m, "/images/curated/book-palestine-embroidery.jpg"),
                new SeedBook("Men in the Sun", "Ghassan Kanafani", "A landmark 1963 novella by one of the most influential Palestinian writers of the 20th century.", 19.00m)
            }),

        new("Syria", "SY", ArabWorld, "Damascene craft heritage: mother-of-pearl inlay, brocade weaving, and centuries-old soap making.",
            Home: new[]
            {
                new SeedProduct("Damascene Inlaid Jewelry Box", "Wooden box inlaid with mother-of-pearl in the historic Damascene technique.", 64.00m),
                new SeedProduct("Aleppo Laurel Soap (Gift Set)", "Traditional Aleppo soap, aged for months using laurel oil in the ancient method.", 26.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Damask Brocade Clutch", "Evening clutch woven from Damask silk brocade, a fabric that takes its name from the city.", 58.00m),
                new SeedProduct("Damascene Filigree Earrings", "Fine silver filigree earrings in a traditional Damascene pattern.", 38.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Damascene Star Phone Case", "Phone case printed with an eight-pointed Damascene inlay star pattern.", 24.00m),
                new SeedProduct("Old Damascus Alley Phone Case", "Phone case featuring the stone archways of Old Damascus.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Aleppo Silk Brocade Kaftan", "Kaftan woven from Aleppo silk brocade in a centuries-old pattern.", 102.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Damascene Embroidered Vest", "Vest finished with traditional Damascene gold-thread embroidery.", 68.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kids' Embroidered Vest", "A child-sized vest finished with a miniature Damascene gold-thread pattern.", 44.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Selected Poems", "Adonis", "A collection by Syria's most influential modern poet and a perennial Nobel Prize contender.", 23.00m, "/images/curated/book-syria-umayyad.jpg"),
                new SeedBook("Arabian Love Poems", "Nizar Qabbani", "Verses by one of the most widely read Arab poets of the 20th century, born in Damascus.", 20.00m, "/images/curated/book-syria-modernhistory.jpg")
            }),

        new("Jordan", "JO", ArabWorld, "Petra-carved heritage: Bedouin weaving, Jerash silverwork, and desert craft traditions.",
            Home: new[]
            {
                new SeedProduct("Petra Sandstone Coaster Set", "Coasters cut from rose-hued sandstone echoing Petra's carved facades.", 32.00m),
                new SeedProduct("Bedouin Woven Rug (Small)", "Hand-loomed wool rug in traditional Bedouin geometric bands.", 86.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Jerash Silver Bracelet", "Hand-engraved silver bracelet in a pattern drawn from Jerash's Roman-Nabatean ruins.", 44.00m),
                new SeedProduct("Bedouin Woven Belt", "Hand-woven wool belt in traditional Bedouin tribal colors.", 32.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Petra Treasury Phone Case", "Phone case featuring Al-Khazneh, the carved facade of Petra.", 24.00m),
                new SeedProduct("Wadi Rum Desert Phone Case", "Phone case featuring the red dunes of Wadi Rum at sunset.", 23.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Bedouin Embroidered Thobe", "Black thobe hand-embroidered with traditional Bedouin cross-stitch.", 110.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Desert Wool Cloak", "Handwoven wool cloak (bisht-style) worn across Jordan's desert regions.", 92.00m, "Men", "One Size"),
                new SeedProduct("Kids' Bedouin Vest", "A child-sized vest hand-embroidered with a simplified Bedouin cross-stitch pattern.", 46.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Diwan of Arar", "Mustafa Wahbi al-Tal", "Verses by Jordan's national poet, 'Arar,' foundational to modern Jordanian literature.", 21.00m, "/images/curated/book-jordan-petra-bourbon.jpg"),
                new SeedBook("Time of White Horses", "Ibrahim Nasrallah", "An International Prize for Arabic Fiction-shortlisted epic of a Levantine village, by a leading Jordanian-Palestinian novelist.", 23.00m, "/images/curated/book-jordan-petra-averbach.jpg")
            }),

        new("Iraq", "IQ", ArabWorld, "Mesopotamian heritage: Baghdad calligraphy, copperware, and one of the world's oldest literary traditions.",
            Home: new[]
            {
                new SeedProduct("Baghdad Hammered Copper Tray", "Hand-hammered copper serving tray in a traditional Mesopotamian pattern.", 58.00m),
                new SeedProduct("Mesopotamian Ziggurat Bookend Set", "Ceramic bookends shaped after ancient Mesopotamian ziggurats.", 36.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Baghdad Calligraphy Scarf", "Silk scarf printed with classical Arabic calligraphy in the Baghdadi style.", 36.00m),
                new SeedProduct("Basra Pearl Hair Comb", "Mother-of-pearl hair comb inspired by Basra's historic pearl trade.", 30.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Cuneiform Script Phone Case", "Phone case printed with ancient Mesopotamian cuneiform script.", 24.00m),
                new SeedProduct("Ziggurat Silhouette Phone Case", "Phone case featuring the silhouette of an ancient Mesopotamian ziggurat.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Baghdadi Embroidered Abaya", "Flowing abaya finished with traditional Baghdadi gold-thread embroidery.", 105.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Kurdish-Iraqi Woven Vest", "Vest handwoven in a pattern drawn from northern Iraq's Kurdish textile tradition.", 74.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kids' Embroidered Tunic", "A child-sized tunic finished with a miniature Baghdadi gold-thread border.", 44.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Frankenstein in Baghdad", "Ahmed Saadawi", "A Booker International-shortlisted novel and winner of the International Prize for Arabic Fiction, set in war-torn Baghdad.", 22.00m, "/images/curated/book-iraq-gilgamesh.jpg"),
                new SeedBook("The Poetry of al-Mutanabbi", "Al-Mutanabbi", "Verses by the 10th-century poet many consider the greatest in the Arabic language.", 20.00m, "/images/curated/book-iraq-7cities.webp")
            }),

        new("Tunisia", "TN", ArabWorld, "Mediterranean heritage: Sidi Bou Said blue-and-white craft, Berber jewelry, and centuries-old ceramics.",
            Home: new[]
            {
                new SeedProduct("Sidi Bou Said Ceramic Plate", "Hand-painted ceramic plate in the signature blue-and-white of Sidi Bou Said.", 34.00m),
                new SeedProduct("Nabeul Pottery Vase", "Hand-thrown vase from Tunisia's historic pottery town of Nabeul.", 46.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Berber Amazigh Necklace", "Silver necklace with coral inlay in a traditional Amazigh (Berber) pattern.", 48.00m),
                new SeedProduct("Chechia Wool Cap", "Hand-felted red wool cap, a centuries-old Tunisian craft tradition.", 32.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Sidi Bou Said Doorway Phone Case", "Phone case featuring the iconic blue doorways of Sidi Bou Said.", 23.00m),
                new SeedProduct("Amazigh Pattern Phone Case", "Phone case printed with a traditional Amazigh geometric motif.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Tunisian Embroidered Jebba", "Traditional jebba tunic finished with silk-thread embroidery.", 88.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kerkennah Woven Wrap", "Lightweight wrap handwoven on the Kerkennah Islands using a traditional loom.", 54.00m, "Women", "One Size"),
                new SeedProduct("Kids' Chechia Cap & Vest Set", "A child-sized felted cap and vest set in the Tunisian chechia tradition.", 40.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("To the Tyrants of the World", "Abu al-Qasim al-Shabbi", "A poetry collection by Tunisia's national poet, whose verses inspired the Arab Spring.", 19.00m, "/images/curated/book-tunisia-crusade.webp"),
                new SeedBook("The Sun's Blood", "Hédi Kaddour", "A celebrated novel by the acclaimed Tunisian-French writer and Prix Goncourt finalist.", 23.00m, "/images/curated/book-tunisia-history.jpg")
            }),

        new("India", "IN", Asia, "South Asian heritage: block printing, brassware, and centuries of handloom weaving.",
            Home: new[]
            {
                new SeedProduct("Block-Print Table Runner", "Hand block-printed cotton table runner from Rajasthan.", 36.00m),
                new SeedProduct("Meenakari Brass Bowl Set", "Hand-painted brass bowls finished with Mughal-era meenakari enamel work.", 52.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Block-Print Cotton Scarf", "Hand block-printed cotton scarf from Rajasthan.", 32.00m),
                new SeedProduct("Brass Filigree Earrings", "Traditional Indian brass filigree earrings.", 18.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Meenakari Pattern Phone Case", "Phone case printed with a hand-painted meenakari floral motif.", 24.00m),
                new SeedProduct("Rajasthan Mandala Phone Case", "Phone case featuring a traditional Rajasthani mandala pattern.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Block-Print Cotton Kurta", "Hand block-printed cotton kurta from Jaipur artisans.", 58.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Banarasi Silk Wedding Saree", "Richly woven silk saree in the Banarasi tradition, finished with gold zari brocade.", 165.00m, "Women", "One Size (5.5m wrap)"),
                new SeedProduct("Kids' Block-Print Kurta", "A child-sized kurta hand block-printed in a simplified Rajasthani pattern.", 34.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Gitanjali", "Rabindranath Tagore", "The 1913 Nobel Prize-winning poetry collection, the first by a non-European laureate.", 18.00m, "/images/curated/book-india-textiles.png"),
                new SeedBook("Malgudi Days", "R.K. Narayan", "A beloved short story collection set in the fictional town of Malgudi, a classic of Indian English literature.", 19.00m, "/images/curated/book-india-designs.jpg")
            }),

        new("Japan", "JP", Asia, "East Asian heritage: washi paper, indigo dye, and minimalist ceramics.",
            Home: new[]
            {
                new SeedProduct("Kintsugi-Style Ceramic Bowl", "Ceramic bowl finished with a gold-seam kintsugi repair pattern.", 58.00m),
                new SeedProduct("Washi Paper Lantern", "Hand-folded lantern made from traditional Japanese washi paper.", 44.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Indigo-Dyed Furoshiki Cloth", "Hand-dyed cotton wrapping cloth in the traditional shibori indigo technique.", 28.00m),
                new SeedProduct("Sakura Blossom Hair Pin", "Lacquered wood hair pin carved in a cherry blossom motif.", 24.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Indigo Washi Phone Cover", "Phone cover featuring traditional Japanese indigo-dyed washi paper design.", 22.00m),
                new SeedProduct("Sakura Blossom Phone Case", "Phone case printed with a hand-illustrated cherry blossom branch.", 27.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Traditional Furisode Kimono", "Long-sleeved furisode kimono in a hand-dyed floral pattern, worn at coming-of-age celebrations.", 210.00m, "Women", "One Size (adjustable with obi)"),
                new SeedProduct("Indigo Noragi Jacket", "Workwear-style jacket hand-dyed with traditional Japanese indigo.", 86.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kids' Yukata Robe", "A child-sized cotton yukata in a playful indigo pattern, easy to wear for festivals.", 58.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("The Tale of Genji", "Murasaki Shikibu", "Often called the world's first novel, written by a Heian-court noblewoman over a thousand years ago.", 26.00m),
                new SeedBook("Snow Country", "Yasunari Kawabata", "A spare, lyrical novel by Japan's first Nobel laureate in Literature.", 21.00m)
            }),

        new("Turkey", "TR", Asia, "Anatolian heritage: Iznik tilework, oya lace, and centuries of kilim weaving.",
            Home: new[]
            {
                new SeedProduct("Iznik Tile Ceramic Bowl", "Hand-painted ceramic bowl in the classic cobalt-and-coral Iznik pattern.", 54.00m),
                new SeedProduct("Anatolian Kilim Cushion Cover", "Cushion cover handwoven in a traditional Anatolian kilim pattern.", 38.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Traditional Turkish Scarf", "Lightweight oya-trimmed scarf woven with a traditional Anatolian border pattern.", 42.00m),
                new SeedProduct("Evil Eye Nazar Bracelet", "Hand-strung bracelet with traditional Anatolian nazar (evil eye) beads.", 22.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Heritage Pattern Phone Case", "Phone case printed with traditional Iznik tile patterns in cobalt and terracotta.", 29.00m),
                new SeedProduct("Nazar Evil Eye Phone Case", "Phone case featuring the traditional Anatolian nazar protective symbol.", 23.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Anatolian Embroidered Vest", "Vest finished with traditional Anatolian silk-thread embroidery.", 78.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Ottoman-Style Kaftan", "Flowing kaftan inspired by Ottoman court dress.", 115.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kids' Embroidered Vest", "A child-sized vest finished with a miniature Anatolian embroidery pattern.", 42.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("My Name is Red", "Orhan Pamuk", "A Nobel Prize-winning novel set among 16th-century Ottoman miniature painters.", 24.00m),
                new SeedBook("Memed, My Hawk", "Yaşar Kemal", "A modern classic of Anatolian village life by one of Turkey's most celebrated novelists.", 22.00m)
            }),

        new("France", "FR", Europe, "French heritage: Provençal textiles, Limoges porcelain, and centuries of craft tradition.",
            Home: new[]
            {
                new SeedProduct("Limoges-Style Porcelain Trinket Box", "Hand-painted porcelain trinket box in the Limoges tradition.", 62.00m),
                new SeedProduct("Provençal Lavender Sachet Set", "Linen sachets filled with lavender from Provence, printed in traditional toile pattern.", 28.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Breton Striped Lace Scarf", "Fine lace scarf woven in the Breton coastal tradition.", 46.00m),
                new SeedProduct("Provençal Print Tote Bag", "Canvas tote printed with a traditional Provençal floral pattern.", 34.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Toile de Jouy Phone Case", "Phone case printed with a classic French toile de Jouy pattern.", 25.00m),
                new SeedProduct("Eiffel Tower Sketch Phone Case", "Phone case featuring a hand-illustrated sketch of the Eiffel Tower.", 23.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Breton Striped Sailor Shirt", "Cotton shirt in the classic Breton stripe, a French maritime heritage staple.", 58.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Provençal Quilted Vest", "Quilted vest made from traditional Provençal boutis fabric.", 84.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Kids' Breton Striped Shirt", "A child-sized cotton shirt in the classic Breton stripe.", 36.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Les Misérables", "Victor Hugo", "One of the most famous novels in world literature, an 1862 epic of justice and redemption.", 24.00m),
                new SeedBook("The Stranger", "Albert Camus", "A landmark existentialist novel by France's Nobel laureate, first published in 1942.", 18.00m)
            }),

        new("Italy", "IT", Europe, "Italian heritage: Murano glass, Deruta ceramics, and Florentine leathercraft.",
            Home: new[]
            {
                new SeedProduct("Murano-Style Glass Bowl", "Hand-blown glass bowl in swirling color, in the Venetian Murano tradition.", 76.00m),
                new SeedProduct("Deruta Hand-Painted Plate", "Ceramic plate hand-painted in the centuries-old Deruta majolica style.", 58.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Florentine Leather Card Holder", "Hand-tooled leather card holder crafted using traditional Florentine techniques.", 42.00m),
                new SeedProduct("Murano Glass Pendant Necklace", "Hand-blown Murano glass pendant on a fine gold-plated chain.", 64.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Venetian Carnival Mask Phone Case", "Phone case featuring a hand-illustrated Venetian carnival mask.", 24.00m),
                new SeedProduct("Tuscan Hills Phone Case", "Phone case featuring the rolling cypress-lined hills of Tuscany.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Sicilian Embroidered Blouse", "Cotton blouse finished with traditional Sicilian folk embroidery.", 68.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Florentine Wool Cape", "Wool cape in a pattern drawn from Florentine Renaissance textile design.", 98.00m, "Men", "One Size"),
                new SeedProduct("Kids' Sicilian Embroidered Romper", "A child-sized romper finished with simplified Sicilian folk embroidery.", 38.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Invisible Cities", "Italo Calvino", "A lyrical novel imagining fantastical cities described to Kublai Khan, a landmark of 20th-century Italian fiction.", 20.00m),
                new SeedBook("The Divine Comedy", "Dante Alighieri", "The 14th-century epic poem that shaped the Italian language itself.", 22.00m)
            }),

        new("Spain", "ES", Europe, "Andalusian heritage: azulejo tilework, flamenco textiles, and Toledo damascene metalwork.",
            Home: new[]
            {
                new SeedProduct("Andalusian Azulejo Coasters", "Hand-painted ceramic coasters in the traditional Andalusian azulejo pattern.", 30.00m),
                new SeedProduct("Toledo Damascene Trinket Box", "Steel trinket box inlaid with gold in the historic Toledo damascene technique.", 66.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Flamenco Fringe Shawl", "Silk shawl with hand-knotted fringe in the flamenco tradition.", 54.00m),
                new SeedProduct("Andalusian Filigree Earrings", "Fine silver filigree earrings in a traditional Andalusian pattern.", 36.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Azulejo Tile Phone Case", "Phone case printed with a traditional Andalusian azulejo tile pattern.", 24.00m),
                new SeedProduct("Flamenco Fan Phone Case", "Phone case featuring a hand-illustrated flamenco fan motif.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Flamenco Ruffle Blouse", "Cotton blouse with traditional flamenco ruffle sleeves.", 62.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Andalusian Embroidered Vest", "Vest finished with traditional Andalusian gold-thread embroidery.", 74.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kids' Flamenco Ruffle Dress", "A child-sized dress with playful flamenco ruffle trim.", 46.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Don Quixote", "Miguel de Cervantes", "Often called the first modern novel, a foundational work of world literature since 1605.", 23.00m),
                new SeedBook("Gypsy Ballads", "Federico García Lorca", "Poetry collection by Spain's most celebrated 20th-century poet, blending Andalusian folklore with modernism.", 19.00m)
            }),

        new("Germany", "DE", Europe, "German heritage: Black Forest woodcraft, Bavarian folk pattern, and centuries of porcelain tradition.",
            Home: new[]
            {
                new SeedProduct("Black Forest Carved Cuckoo Box", "Hand-carved wooden box in the Black Forest woodworking tradition.", 54.00m),
                new SeedProduct("Meissen-Style Porcelain Cup", "Hand-painted porcelain cup in the classic Meissen floral pattern.", 48.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Bavarian Trachten Brooch", "Enamel brooch in a traditional Bavarian Trachten folk pattern.", 28.00m),
                new SeedProduct("Black Forest Wool Scarf", "Wool scarf woven in a traditional Black Forest tartan pattern.", 38.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Bavarian Folk Pattern Phone Case", "Phone case printed with a traditional Bavarian folk floral motif.", 23.00m),
                new SeedProduct("Neuschwanstein Castle Phone Case", "Phone case featuring the fairytale silhouette of Neuschwanstein Castle.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Bavarian Dirndl-Style Blouse", "Cotton blouse in the traditional Bavarian dirndl style.", 68.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Black Forest Wool Vest", "Wool vest finished with traditional Black Forest folk embroidery.", 76.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Kids' Bavarian Lederhosen-Style Shorts", "Child-sized shorts in the traditional Bavarian style, with folk-pattern trim.", 44.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("Faust", "Johann Wolfgang von Goethe", "Germany's most celebrated literary work, a philosophical drama first published in 1808.", 22.00m),
                new SeedBook("Siddhartha", "Hermann Hesse", "A spiritual classic by Germany's Nobel laureate in Literature, first published in 1922.", 18.00m)
            }),

        new("United States", "US", Americas, "American heritage: Americana folk art, quilting tradition, and regional craft from coast to coast.",
            Home: new[]
            {
                new SeedProduct("Americana Folk Art Quilt Square", "Hand-stitched quilt square in a traditional American folk art pattern.", 48.00m),
                new SeedProduct("Appalachian Carved Wood Bowl", "Hand-carved wooden bowl in the Appalachian woodworking tradition.", 42.00m)
            },
            Accessories: new[]
            {
                new SeedProduct("Southwestern Turquoise Bracelet", "Silver bracelet set with turquoise, in a Southwestern craft tradition.", 56.00m),
                new SeedProduct("Denim Patchwork Tote", "Tote bag stitched from reclaimed denim in a classic Americana patchwork pattern.", 38.00m)
            },
            PhoneCovers: new[]
            {
                new SeedProduct("Americana Quilt Pattern Phone Case", "Phone case printed with a traditional American patchwork quilt pattern.", 23.00m),
                new SeedProduct("Route 66 Phone Case", "Phone case featuring vintage Route 66 highway signage art.", 22.00m)
            },
            Wear: new[]
            {
                new SeedProduct("Denim Heritage Jacket", "Classic American denim jacket in a heritage workwear cut.", 88.00m, "Men", "S,M,L,XL,XXL"),
                new SeedProduct("Quilted Patchwork Vest", "Vest stitched from a traditional American patchwork quilt pattern.", 64.00m, "Women", "XS,S,M,L,XL"),
                new SeedProduct("Kids' Denim Patchwork Overalls", "Child-sized overalls stitched with a playful Americana patchwork panel.", 42.00m, "Kids", "2T,4T,6,8,10,12")
            },
            Books: new[]
            {
                new SeedBook("The Adventures of Huckleberry Finn", "Mark Twain", "An 1884 classic often called the first great American novel.", 19.00m),
                new SeedBook("Beloved", "Toni Morrison", "A Pulitzer Prize and Nobel Prize-winning novel, among the most acclaimed works of American literature.", 20.00m)
            })
    };
}
