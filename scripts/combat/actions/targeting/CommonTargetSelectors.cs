namespace Combat {
    public static class CommonSelectors {
        public static Selector Melee => new Selector () {
            Type = TargetType.Single,
            Side = SideSelector.Opposite,
            Row = 0,
            VerticalRange = 2,
        };

        public static Selector Enemy => new Selector () {
            Type = TargetType.Single,
            Side = SideSelector.Opposite,
        };
    }
}