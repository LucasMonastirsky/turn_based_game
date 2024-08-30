namespace Combat {
    public static class CommonTargetSelectors {
        public static Selector Melee => new Selector() {
            Type = TargetType.Single,
            Side = SideSelector.Opposite,
            Row = 0,
            VerticalRange = 1,
        };
    }
}