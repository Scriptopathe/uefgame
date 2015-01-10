#uefinitialize {
	GameEvent evt = (GameEvent)self;
	var commandset = new CommandSet();
	commandset.Id = 1;
	commandset.Conditions += CommandSet.Conditions.Switch("A", false);
	commandset.Conditions += CommandSet.Conditions.PlayerTouched;
	
	// Script
	commandset.Conditions += delegate(GameEvent evt, CommandSet cmdSet)
	{
		return true;
	};
	// ex de delegate
	commandset.Actions.Add(new CommandSetAction(delegate(GameEvent evt, CommandSet cmdSet)
	{
		
	}));
	
	
	// Ex de script
	commandset.Actions.Add(CommandSet.Actions.MoveLeft(8));
	commandset.Actions.Add(CommandSet.Actions.Jump());
	commandset.Actions.Add(new CommandSetIf(
	// if
	delegate(GameEvent evt, CommandSet cmdSet)
	{
		return true;
	},
	// -> if true
	new CommandSetActionList() {
		CommandSet.Actions.Message("HAHAHA"),
		CommandSet.Actions.MoveLeft(8),
	}));
	/*
	@if $Switch("A");
		@MoveLeft;
		@Suck;
	@elsif $Caca("B");
		@Fuck
	@else;
		@MoveRight;
		@DontSuck;
	@endif;
	
	if $Switch
		@MoveLeft
		@Suck
	else
		if $Caca
			@Fuck
		else
			@MoveRight
			@DontSuck
	*/
	/* Rules :
	#script 		=> new CommandActionList() {
	@Bla(jidejioe) 	=> CommandSet.Actions.Bla(jidejioe)
	; 				=> ,
	$Truc 			=> CommandSet.Conditions.Truc
	----------------
	FlOW :
	
	if 				=> CommandSet.Flow.If(
	blabla 			=> blabla
	{  				=> , new CommandSetActionList() {
	}				=> }
	else			=> ,
	{				=> new CommandSetActionList() {
	
	}				=> });
	*/
	// Equivalent
}