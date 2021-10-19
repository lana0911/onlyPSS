import jieba
import random
import time
import json

random.seed(time.time())

with open('momo.json',"r",encoding="utf-8") as json_data:
    dict = json.load(json_data)
    

def predictIntent(word_list):
    for word in word_list:
        for dictCnt in range(0, len(dict)):
            for utterance in dict[dictCnt]['utterances']:
                if word == utterance:
                    return dict[dictCnt]['intent']

    return "Unknown Intent"


def Intent2Answer(input_intent):
    right_intent_dict_index = -1
    for dictCnt in range(0, len(dict)):
        if input_intent == dict[dictCnt]['intent']:
            right_intent_dict_index = dictCnt
            break

    answerNum = len(dict[right_intent_dict_index]['answers'])
    return dict[right_intent_dict_index]['answers'][random.randint(0, answerNum-1)]
while True:
    print("Q:")
    seg_list = jieba.lcut(input(), cut_all=True)

    print("|".join(seg_list))

    intent = predictIntent(seg_list)
    print(intent)

    answer = Intent2Answer(intent)

    print("回覆: ", answer)
