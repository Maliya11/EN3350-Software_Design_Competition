package com.project.backend.controller;

import com.project.backend.entity.Player;
import com.project.backend.service.PlayerService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("player")
@CrossOrigin
public class PlayerController {

    @Autowired
    PlayerService playerService;

    @PostMapping("/authenticate")
    public Map<String,Object> playerStateIdentify(@RequestBody Map<String, String> requestBody){
        //set all player states to zero(as not playing)
        playerService.setPlayerStatesToZero();

        //get the apiKey seperately as a string
        String apiKey = requestBody.get("apiKey");

        //Identify or register player and set player state as active(playing)
        boolean validKey = playerService.playerStateIdentify(apiKey);
        Player activePlayer = playerService.identifyActivePlayer();

        //Send to unity whether the apiKey is valid and number of questions that the player have completed
        Integer completedQuestions = activePlayer.getCompletedQuestions();
        Map<String, Object> response = new HashMap<>();
        response.put("validKey", validKey);
        response.put("completedQuestions", completedQuestions);
        return response;
    }

    @GetMapping("/details")
    public Player sendPlayerDetails(){
        //getting the details of the current player
        return playerService.identifyActivePlayer();
    }


    @PostMapping("/answer")
    public void playerAnswerSubmit(@RequestBody AnswerUpdateRequest request){
        //identify the active player
        Player player = playerService.identifyActivePlayer();

        //save the submitted answer in the player table
        playerService.playerAnswerSubmit(request.getqNum(), request.getSelAns());

        //increment the completed questions of the player by one
        playerService.incrementCompletedQuestion(player);
    }

    @PostMapping("/bonus")
    public void playerBonusGiven(@RequestBody Map<String, Integer> requestBody) {
        // Extract bonusGiven value from request body
        Integer bonusGiven = requestBody.get("bonusGiven");

        // Update the player in the database
        playerService.playerBonusGiven(bonusGiven);
    }


}
