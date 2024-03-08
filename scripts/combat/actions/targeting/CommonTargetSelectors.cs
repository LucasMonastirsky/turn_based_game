namespace Combat {
    public static class CommonTargetSelectors {
        public static TargetSelector Melee => new TargetSelector() {
            Type = TargetType.Single,
            Side = SideSelector.Opposite,
            Row = 0,
            VerticalRange = 1,
        };
    }
}