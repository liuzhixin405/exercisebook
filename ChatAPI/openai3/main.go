package main

import (
	"fmt"
	"log"
	"time"

	"github.com/chatgp/gpt3"
)

func main() {
	chatImages()
	/* 	apiKey := "xxxxxxxxxxxxxxxx"
	   	cli, _ := gpt3.NewClient(&gpt3.Options{
	   		ApiKey:  apiKey,
	   		Timeout: 30 * time.Second,
	   		Debug:   true,
	   	})
	   	uri := "/v1/models"
	   	res, err := cli.Get(uri)
	   	if err != nil {
	   		log.Fatal("request api failed:%v", err)
	   	}
	   	for _, v := range res.Get("data").Array() {
	   		fmt.Printf("model id: %s\n", v.Get("id").String())
	   	} */
}

func getClient() *gpt3.Client {
	apiKey := "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
	cli, _ := gpt3.NewClient(&gpt3.Options{
		ApiKey:  apiKey,
		Timeout: 30 * time.Second,
		Debug:   true,
	})
	return cli
}

func chatTurbo() {
	cli := getClient()

	uri := "/v1/chat/completions"
	params := map[string]interface{}{
		"model": "gpt-3.5-turbo",
		"messages": []map[string]interface{}{
			{"role": "user", "content": "hello"},
		},
	}

	res, err := cli.Post(uri, params)
	if err != nil {
		log.Fatalf("request api failed: %v", err)
	}

	message := res.Get("choices.0.message.content").String()

	fmt.Printf("message is: %s", message)
}

func chatDavinci() {
	cli := getClient()
	uri := "/v1/completions"
	params := map[string]interface{}{
		"model":       "text-davinci-003",
		"prompt":      "say hello three times",
		"max_tokens":  2048,
		"temperature": 0.9,
		"n":           1,
		"stream":      false,
	}

	res, err := cli.Post(uri, params)

	if err != nil {
		log.Fatalf("request api failed: %v", err)
	}

	fmt.Println(res.GetString("choices.0.text"))
}

func chatDavinciEdit() {
	cli := getClient()
	uri := "/v1/edits"
	params := map[string]interface{}{
		"model":       "text-davinci-edit-001",
		"input":       "Are you hapy today?",
		"instruction": "fix mistake",
		"temperature": 0.9,
		"n":           1,
	}

	res, err := cli.Post(uri, params)

	if err != nil {
		log.Fatalf("request api failed: %v", err)
	}

	fmt.Println(res.GetString("choices.0.text"))
}

func chatImages() {
	cli := getClient()
	uri := "/v1/images/generations"
	params := map[string]interface{}{
		"prompt":          "a beautiful girl with big eyes",
		"n":               1,
		"size":            "256x256",
		"response_format": "url",
	}

	res, err := cli.Post(uri, params)

	if err != nil {
		log.Fatalf("request api failed: %v", err)
	}

	fmt.Println(res.GetString("data.0.url"))
}
