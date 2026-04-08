extends Node2D

@export var enemy_scene: PackedScene

var score = 0

@onready var score_label = $UI/ScoreLabel

func _ready():
	update_score()

func _on_spawn_timer_timeout():
	var enemy = enemy_scene.instantiate()
	
	enemy.global_position = Vector2(
		randf_range(50, 1100),
		randf_range(50, 600)
	)
	
	add_child(enemy)

func add_score(amount):
	score += amount
	update_score()

func update_score():
	score_label.text = "Score: " + str(score)
