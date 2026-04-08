extends Node2D

@export var enemy_scene: PackedScene

var score = 0
var game_over = false

@onready var score_label = $UI/ScoreLabel
@onready var game_over_panel = $UI/GameOverPanel
@onready var final_score_label = $UI/GameOverPanel/VBoxContainer/FinalScoreLabel
@onready var spawn_timer = $SpawnTimer

func _ready():
	update_score()
	game_over_panel.visible = false

func _on_spawn_timer_timeout():
	if game_over:
		return
	
	var enemy = enemy_scene.instantiate()
	enemy.global_position = Vector2(
		randf_range(50, 1100),
		randf_range(50, 600)
	)
	add_child(enemy)

func add_score(amount):
	if game_over:
		return
	
	score += amount
	update_score()

func update_score():
	score_label.text = "Score: " + str(score)

func show_game_over():
	game_over = true
	spawn_timer.stop()
	final_score_label.text = "Score: " + str(score)
	game_over_panel.visible = true

func _on_restart_button_pressed():
	get_tree().reload_current_scene()

func _on_menu_button_pressed():
	get_tree().change_scene_to_file("res://main_menu.tscn")
