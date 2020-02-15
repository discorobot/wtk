namespace wtk
{
    public class ManuscriptFileMetadata {
        public string FullPath {get;set;}
        public int? Chapter {get;set;}
        public int? Part {get;set;}
        public int Words {get;set;}

        public override string ToString(){
            return $"part {Part} chapter {Chapter} FullPath {FullPath}";
        }

    }
}